#!/bin/sh
set -eu

public_host="${DEMO_PUBLIC_HOST:-localhost}"
angular_port="${ANGULAR_PORT:-8084}"
wasm_port="${BLAZOR_WASM_PORT:-8083}"
angular_origin="http://${public_host}:${angular_port}"
wasm_origin="http://${public_host}:${wasm_port}"

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
  webapp: "${BLAZOR_WEBAPP_PORT:-8081}",
  server: "${BLAZOR_SERVER_PORT:-8082}",
  wasm: "${wasm_port}",
  angular: "${angular_port}"
};
EOF

echo "Applying FileClerk demo migrations and seed data..."
cd /app/migrator
dotnet Starbender.FileClerk.Demo.DbMigrator.dll

echo "Migrations and seed data completed; starting demo hosts."
exec /usr/bin/supervisord --configuration /etc/supervisor/supervisord.conf
