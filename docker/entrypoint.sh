#!/bin/sh
set -eu

public_host="${DEMO_PUBLIC_HOST:-localhost}"
landing_port="${LANDING_PORT:-8080}"
webapp_port="${BLAZOR_WEBAPP_PORT:-8081}"
server_port="${BLAZOR_SERVER_PORT:-8082}"
angular_port="${ANGULAR_PORT:-8084}"
wasm_port="${BLAZOR_WASM_PORT:-8083}"
angular_origin="http://${public_host}:${angular_port}"
wasm_origin="http://${public_host}:${wasm_port}"

case "$public_host" in
    *[!A-Za-z0-9.-]*)
        echo "DEMO_PUBLIC_HOST must be a hostname or IPv4 address: $public_host" >&2
        exit 64
        ;;
esac

for public_port in "$landing_port" "$webapp_port" "$server_port" "$wasm_port" "$angular_port"; do
    case "$public_port" in
        ''|*[!0-9]*)
            echo "Demo public ports must be numeric: $public_port" >&2
            exit 64
            ;;
    esac
done

sed \
    -e "s|__DEMO_PUBLIC_HOST__|${public_host}|g" \
    -e "s|__LANDING_PORT__|${landing_port}|g" \
    -e "s|__BLAZOR_WEBAPP_PORT__|${webapp_port}|g" \
    -e "s|__BLAZOR_SERVER_PORT__|${server_port}|g" \
    -e "s|__BLAZOR_WASM_PORT__|${wasm_port}|g" \
    -e "s|__ANGULAR_PORT__|${angular_port}|g" \
    /etc/nginx/nginx.conf > /tmp/nginx/nginx.conf
nginx -t -c /tmp/nginx/nginx.conf

export OpenIddict__Applications__FileClerkDemo_Angular__RootUrl="$angular_origin"
export OpenIddict__Applications__FileClerkDemo_BlazorWebAssembly__RootUrl="$wasm_origin"
export OpenIddict__Applications__FileClerkDemo_AngularSwagger__RootUrl="$angular_origin"
export OpenIddict__Applications__FileClerkDemo_BlazorWebAssemblySwagger__RootUrl="$wasm_origin"
export StringEncryption__DefaultPassPhrase="${DEMO_ENCRYPTION_PASSPHRASE:-fileclerk-demo-string-encryption}"

cat > /app/angular/dynamic-env.json <<EOF
{
  "application": { "baseUrl": "${angular_origin}" },
  "oAuthConfig": {
    "issuer": "${angular_origin}/",
    "redirectUri": "${angular_origin}",
    "requireHttps": false
  },
  "apis": {
    "default": { "url": "${angular_origin}" },
    "FileClerk": { "url": "${angular_origin}" },
    "AbpAccountPublic": { "url": "${angular_origin}/" }
  }
}
EOF

cat > /app/landing/demo-config.js <<EOF
window.fileClerkDemoPorts = {
  webapp: "${webapp_port}",
  server: "${server_port}",
  wasm: "${wasm_port}",
  angular: "${angular_port}"
};
window.fileClerkDemoPublicHost = "${public_host}";
EOF

echo "Applying FileClerk demo migrations and seed data..."
cd /app/migrator
dotnet Starbender.FileClerk.Demo.DbMigrator.dll

echo "Migrations and seed data completed; starting demo hosts."
exec /usr/bin/supervisord --configuration /etc/supervisor/supervisord.conf
