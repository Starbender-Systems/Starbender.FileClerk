FROM mcr.microsoft.com/dotnet/sdk:10.0.302-noble AS dotnet-build

WORKDIR /workspace

COPY global.json ./
COPY src ./src

RUN dotnet restore src/Starbender.FileClerk.slnx \
    && dotnet build src/Starbender.FileClerk.slnx \
        --configuration Release \
        --no-restore \
    && touch /tmp/dotnet-build-complete


FROM node:26.5.1-bookworm-slim AS angular-build

WORKDIR /workspace/src/angular

COPY src/angular/package.json src/angular/package-lock.json ./
RUN npm ci

COPY src/angular ./
RUN npm run build \
    && touch /tmp/angular-build-complete


FROM mcr.microsoft.com/dotnet/aspnet:10.0.10-noble AS runtime

RUN apt-get update \
    && apt-get install --yes --no-install-recommends \
        curl \
        nginx \
        supervisor \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir --parents \
        /app/landing \
        /opt/fileclerk/build-info \
        /tmp/nginx/client-body \
        /tmp/nginx/fastcgi \
        /tmp/nginx/proxy \
        /tmp/nginx/scgi \
        /tmp/nginx/uwsgi \
        /tmp/supervisor \
    && chown --recursive app:app \
        /app \
        /opt/fileclerk \
        /tmp/nginx \
        /tmp/supervisor

COPY --from=dotnet-build /tmp/dotnet-build-complete /opt/fileclerk/build-info/
COPY --from=angular-build /tmp/angular-build-complete /opt/fileclerk/build-info/
COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY docker/supervisord.conf /etc/supervisor/supervisord.conf
COPY docker/landing /app/landing

RUN nginx -t \
    && rm --force /tmp/nginx/nginx.pid \
    && chown --recursive app:app \
        /app/landing \
        /opt/fileclerk/build-info \
        /tmp/nginx

USER app

EXPOSE 8080 8081 8082 8083 8084

ENTRYPOINT ["/usr/bin/supervisord", "--configuration", "/etc/supervisor/supervisord.conf"]
