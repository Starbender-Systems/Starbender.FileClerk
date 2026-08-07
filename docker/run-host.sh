#!/bin/sh
set -eu

role="$1"
public_host="${DEMO_PUBLIC_HOST:-localhost}"
angular_port="${ANGULAR_PORT:-8084}"
wasm_port="${BLAZOR_WASM_PORT:-8083}"
angular_origin="http://${public_host}:${angular_port}"
wasm_origin="http://${public_host}:${wasm_port}"

export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
export AuthServer__CertificatePassPhrase=fileclerk-demo-openiddict
export StringEncryption__DefaultPassPhrase="${DEMO_ENCRYPTION_PASSPHRASE:-fileclerk-demo-string-encryption}"

case "$role" in
    api-angular)
        origin="$angular_origin"
        health_origin=http://127.0.0.1:5104
        startup_url=${health_origin}/health-status
        export ASPNETCORE_URLS=http://0.0.0.0:5104
        export AuthServer__SwaggerClientId=FileClerkDemo_AngularSwagger
        app_dir=/app/api
        app_dll=Starbender.FileClerk.Demo.HttpApi.Host.dll
        ;;
    api-wasm)
        origin="$wasm_origin"
        health_origin=http://127.0.0.1:5103
        startup_url=${health_origin}/health-status
        export ASPNETCORE_URLS=http://0.0.0.0:5103
        export AuthServer__SwaggerClientId=FileClerkDemo_BlazorWebAssemblySwagger
        app_dir=/app/api
        app_dll=Starbender.FileClerk.Demo.HttpApi.Host.dll
        ;;
    blazor-webapp)
        origin="http://${public_host}:${BLAZOR_WEBAPP_PORT:-8081}"
        health_origin=""
        startup_url=http://127.0.0.1:5001/
        export ASPNETCORE_URLS=http://0.0.0.0:5001
        app_dir=/app/blazor-webapp
        app_dll=Starbender.FileClerk.Demo.Blazor.dll
        ;;
    blazor-server)
        origin="http://${public_host}:${BLAZOR_SERVER_PORT:-8082}"
        health_origin=http://127.0.0.1:5002
        startup_url=${health_origin}/health-status
        export ASPNETCORE_URLS=http://0.0.0.0:5002
        app_dir=/app/blazor-server
        app_dll=Starbender.FileClerk.Demo.BlazorServer.Blazor.dll
        ;;
    blazor-wasm)
        export ASPNETCORE_URLS=http://0.0.0.0:5003
        cd /app/blazor-wasm
        exec dotnet Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.dll
        ;;
    mvc)
        origin="http://${public_host}:${MVC_PORT:-8085}"
        health_origin=http://127.0.0.1:5005
        startup_url=${health_origin}/health-status
        export ASPNETCORE_URLS=http://0.0.0.0:5005
        app_dir=/app/mvc
        app_dll=Starbender.FileClerk.Demo.Web.dll
        ;;
    *)
        echo "Unknown host role: $role" >&2
        exit 64
        ;;
esac

export App__SelfUrl="$origin"
export App__DemoUrl="$origin"
export App__CorsOrigins="${angular_origin},${wasm_origin}"
export App__RedirectAllowedUrls="${angular_origin},${wasm_origin}"
export AuthServer__Authority="$origin"
export AuthServer__RequireHttpsMetadata=false

if [ -n "$health_origin" ]; then
    export App__HealthCheckUrl="${health_origin}/health-status"
    export App__HealthUiCheckUrl="${health_origin}/health-status"
fi

cd "$app_dir"

# ABP persists permission, feature, and setting definitions while a host is
# initialized. Serialize the database-backed hosts on a fresh database to avoid
# unique-key races; after the endpoint is ready, every host runs concurrently.
startup_lock=/tmp/fileclerk-demo-host-startup.lock
while ! mkdir "$startup_lock" 2>/dev/null; do
    sleep 0.2
done

dotnet "$app_dll" &
child_pid=$!

cleanup_startup_lock() {
    rmdir "$startup_lock" 2>/dev/null || true
}
trap cleanup_startup_lock EXIT

attempt=0
until curl --fail --silent --show-error "$startup_url" >/dev/null 2>&1; do
    if ! kill -0 "$child_pid" 2>/dev/null; then
        wait "$child_pid"
        exit $?
    fi

    attempt=$((attempt + 1))
    if [ "$attempt" -ge 900 ]; then
        echo "Timed out waiting for $role to initialize." >&2
        kill "$child_pid" 2>/dev/null || true
        wait "$child_pid" || true
        exit 1
    fi
    sleep 0.2
done

cleanup_startup_lock
trap - EXIT

terminate_child() {
    kill -TERM "$child_pid" 2>/dev/null || true
    wait "$child_pid" || true
    exit 0
}
trap terminate_child INT TERM
wait "$child_pid"
