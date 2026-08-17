FROM node:26.7.0-bookworm-slim AS node-base


FROM mcr.microsoft.com/dotnet/sdk:10.0.302-noble AS dotnet-build

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    PATH="/tools:${PATH}"

COPY --from=node-base /usr/local/bin/node /usr/local/bin/node
COPY --from=node-base /usr/local/lib/node_modules /usr/local/lib/node_modules
RUN ln -s /usr/local/lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm \
    && ln -s /usr/local/lib/node_modules/npm/bin/npx-cli.js /usr/local/bin/npx

WORKDIR /workspace
COPY global.json ./
COPY src ./src

RUN dotnet restore src/Starbender.FileClerk.slnx \
    && dotnet restore src/demo/Starbender.FileClerk.Demo.slnx \
    && dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore \
    && dotnet build src/demo/Starbender.FileClerk.Demo.slnx --configuration Release --no-restore \
    && dotnet test src/Starbender.FileClerk.slnx --configuration Release --no-build \
    && dotnet test src/demo/Starbender.FileClerk.Demo.slnx --configuration Release --no-build

RUN dotnet tool install --tool-path /tools Volo.Abp.Studio.Cli --version 3.0.8 \
    && cd /workspace/src/demo/shared/Starbender.FileClerk.Demo.HttpApi.Host \
    && abp install-libs \
    && cd /workspace/src/demo/blazor-webapp/Starbender.FileClerk.Demo.Blazor \
    && abp install-libs \
    && cd /workspace/src/demo/blazor-server/Starbender.FileClerk.Demo.BlazorServer \
    && abp install-libs \
    && cd /workspace/src/demo/mvc/Starbender.FileClerk.Demo.Web \
    && abp install-libs

RUN dotnet dev-certs https \
        --export-path /tmp/openiddict.pfx \
        --password fileclerk-demo-openiddict \
    && dotnet publish src/demo/shared/Starbender.FileClerk.Demo.DbMigrator/Starbender.FileClerk.Demo.DbMigrator.csproj \
        --configuration Release --no-restore --output /out/migrator \
    && dotnet publish src/demo/shared/Starbender.FileClerk.Demo.HttpApi.Host/Starbender.FileClerk.Demo.HttpApi.Host.csproj \
        --configuration Release --no-restore --output /out/api \
    && dotnet publish src/demo/blazor-webapp/Starbender.FileClerk.Demo.Blazor/Starbender.FileClerk.Demo.Blazor.csproj \
        --configuration Release --no-restore --output /out/blazor-webapp \
    && dotnet publish src/demo/blazor-server/Starbender.FileClerk.Demo.BlazorServer/Starbender.FileClerk.Demo.BlazorServer.Blazor.csproj \
        --configuration Release --no-restore --output /out/blazor-server \
    && dotnet publish src/demo/blazor-webassembly/Starbender.FileClerk.Demo.BlazorWebAssembly/Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.csproj \
        --configuration Release --no-restore --output /out/blazor-wasm \
    && dotnet publish src/demo/mvc/Starbender.FileClerk.Demo.Web/Starbender.FileClerk.Demo.Web.csproj \
        --configuration Release --no-restore --output /out/mvc \
    && cp /tmp/openiddict.pfx /out/api/openiddict.pfx \
    && cp /tmp/openiddict.pfx /out/blazor-webapp/openiddict.pfx \
    && cp /tmp/openiddict.pfx /out/blazor-server/openiddict.pfx \
    && cp /tmp/openiddict.pfx /out/mvc/openiddict.pfx


FROM node-base AS angular-build

WORKDIR /workspace/src/angular
COPY src/angular/package.json src/angular/package-lock.json ./
RUN npm ci
COPY src/angular ./
RUN npm run lint && npm run build

WORKDIR /workspace/src/demo/angular
COPY src/demo/angular/package.json src/demo/angular/package-lock.json src/demo/angular/.npmrc ./
RUN npm ci
COPY src/demo/angular ./
RUN npm run lint && npm run build:prod


FROM mcr.microsoft.com/dotnet/aspnet:10.0.11-noble AS runtime

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl nginx supervisor \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir --parents \
        /app/api/Logs \
        /app/angular \
        /app/bin \
        /app/blazor-server/Logs \
        /app/blazor-wasm/Logs \
        /app/blazor-webapp/Logs \
        /app/landing \
        /app/migrator/Logs \
        /app/mvc/Logs \
        /home/app/.aspnet/DataProtection-Keys \
        /tmp/nginx/client-body \
        /tmp/nginx/fastcgi \
        /tmp/nginx/proxy \
        /tmp/nginx/scgi \
        /tmp/nginx/uwsgi \
        /tmp/supervisor \
    && chown --recursive app:app /app /home/app/.aspnet /tmp/nginx /tmp/supervisor

COPY --chown=app:app --from=dotnet-build /out/api /app/api
COPY --chown=app:app --from=dotnet-build /out/blazor-server /app/blazor-server
COPY --chown=app:app --from=dotnet-build /out/blazor-wasm /app/blazor-wasm
COPY --chown=app:app --from=dotnet-build /out/blazor-webapp /app/blazor-webapp
COPY --chown=app:app --from=dotnet-build /out/migrator /app/migrator
COPY --chown=app:app --from=dotnet-build /out/mvc /app/mvc
COPY --chown=app:app --from=angular-build /workspace/src/demo/angular/dist/Angular/browser /app/angular
COPY --chown=app:app docker/landing /app/landing
COPY --chown=app:app \
    src/demo/shared/Starbender.FileClerk.Demo.HttpApi.Host/wwwroot/images/clients/angular.svg \
    src/demo/shared/Starbender.FileClerk.Demo.HttpApi.Host/wwwroot/images/clients/aspnetcore.svg \
    src/demo/shared/Starbender.FileClerk.Demo.HttpApi.Host/wwwroot/images/clients/blazor.svg \
    /app/landing/images/clients/
COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY docker/supervisord.conf /etc/supervisor/supervisord.conf
COPY --chmod=755 docker/entrypoint.sh docker/healthcheck.sh docker/run-host.sh /app/bin/

RUN nginx -t \
    && rm --force /tmp/nginx/nginx.pid \
    && chown --recursive app:app /tmp/nginx

USER app

EXPOSE 8080 8081 8082 8083 8084 8085

HEALTHCHECK --interval=10s --timeout=5s --start-period=90s --retries=12 CMD ["/app/bin/healthcheck.sh"]
ENTRYPOINT ["/app/bin/entrypoint.sh"]
