# Starbender.FileClerk

Starbender.FileClerk is an [ABP Framework](https://abp.io/) module for managing
BLOB storage containers and their provider configuration.

> [!IMPORTANT]
> FileClerk is under active development. No NuGet or npm packages have been
> released, and the current repository is not production ready.

## Compatibility

| Component | Version |
| --- | --- |
| .NET SDK | 10.0.300 or a later 10.0.3xx patch |
| ABP Framework | 10.6.0 |
| Angular | 22.1.x |
| Node.js | 26.5.1 |

The repository pins the .NET and Node.js toolchains through `global.json` and
`.nvmrc`.

## Build from a clean clone

Build and test the reusable module:

```bash
dotnet restore src/Starbender.FileClerk.slnx
dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore
dotnet test src/Starbender.FileClerk.slnx --configuration Release --no-build

cd src/angular
npm ci
npm run lint
npm run build
```
The independently buildable demo solution includes the shared backend, all
four .NET UI hosts, and the standard ABP test projects:

## NuGet packages and versions

The 13 projects under `src/src` are packaged together and always use the same
version. Test projects are not packable. Package metadata, the repository
README, and `src/src/Nuget.png` are added centrally to every package.

Restore the pinned GitVersion tool and inspect the current release line with:

```bash
dotnet tool restore
dotnet gitversion /showvariable MajorMinorPatch
```

For a local package build, choose a valid prerelease version and run:

```bash
dotnet restore src/Starbender.FileClerk.slnx
dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore \
  -p:Version=0.1.0-prerelease-local -p:PackageVersion=0.1.0-prerelease-local
dotnet pack src/Starbender.FileClerk.slnx --configuration Release \
  --no-build --no-restore \
  -p:Version=0.1.0-prerelease-local \
  -p:PackageVersion=0.1.0-prerelease-local \
  -p:PackageOutputPath=artifacts/nuget
./scripts/validate-nuget-packages.sh artifacts/nuget 0.1.0-prerelease-local
```

CI uses the GitVersion `MajorMinorPatch` value as the base. Pull requests and
untagged `main` builds produce `-ci-<run-number>` artifacts. Pushes to
`development` produce downloadable `-prerelease-<run-number>` artifacts and
never receive NuGet.org credentials. Stable packages are published only for an
exact `vMajor.Minor.Fix` tag whose commit is reachable from `main`.

After the first stable release, packages can be installed from NuGet.org, for
example:

```bash
dotnet add package Starbender.FileClerk.Domain.Shared --version 0.1.0
```

The release workflow is:

1. Merge normal changes into `development`.
2. Promote `development` to `main` with a release pull request.
3. Tag the selected `main` commit as `vMajor.Minor.Fix` and push the tag.
4. Verify all 13 package pages and their metadata on NuGet.org.

An ordinary squash merge advances the fix version. Put `+semver: minor` or
`+semver: major` in the squash commit message when that larger increment is
intentional; `+semver: fix` is also supported. The NuGet.org API key is stored
as the organization-level `NUGET_API_KEY` Actions secret and must grant this
repository access. The `nuget.org` environment restricts its use to release-tag
jobs. The key must be scoped to `Starbender.FileClerk.*` packages and rotated
before expiry.

## Demo containers

```bash
dotnet restore src/demo/Starbender.FileClerk.Demo.slnx
dotnet build src/demo/Starbender.FileClerk.Demo.slnx --configuration Release --no-restore
dotnet test src/demo/Starbender.FileClerk.Demo.slnx --configuration Release --no-build

cd src/demo/angular
npm ci
npm run lint
npm run build:prod
```

## Demo applications

Docker Compose is the supported way to run the demos. It builds one non-root
application image and starts it beside PostgreSQL. The entrypoint runs the
single shared DbMigrator synchronously before Supervisor starts any web host.

```bash
docker compose up --build --detach --wait
docker compose ps
docker compose logs --follow app
```

| URL | Application |
| --- | --- |
| <http://localhost:8080> | Demo landing page |
| <http://localhost:8081> | Blazor WebApp |
| <http://localhost:8082> | Blazor Server |
| <http://localhost:8083> | Blazor WebAssembly and its same-origin API/auth proxy |
| <http://localhost:8084> | Angular and its same-origin API/auth proxy |
| <http://localhost:8085> | MVC/Razor Pages |

Use the template-seeded administrator account:

```text
Username: admin
Password: 1q2w3E*
```

Every application uses the ABP Basic Theme and the same PostgreSQL schema and
data. The two static clients have separate OpenIddict registrations and run
against separate instances of the shared HTTP API host so each authority stays
same-origin. Both `Default` and `FileClerk` remote services and connection
strings resolve to the shared backend.

### Configuration

Copy `.env.example` to `.env` to persist overrides. Defaults are intended only
for local manual testing.

| Variable | Default | Purpose |
| --- | --- | --- |
| `POSTGRES_DB` | `fileclerk` | Shared database name |
| `POSTGRES_USER` | `fileclerk` | Database user |
| `POSTGRES_PASSWORD` | `fileclerk_dev_only` | Local-only database password |
| `DATABASE_BIND_ADDRESS` | `127.0.0.1` | PostgreSQL host bind address |
| `POSTGRES_PORT` | `5432` | PostgreSQL host port |
| `APP_BIND_ADDRESS` | `127.0.0.1` | Web application bind address |
| `DEMO_PUBLIC_HOST` | `localhost` | Browser-visible hostname used in issuer and callback URLs |
| `DEMO_ENCRYPTION_PASSPHRASE` | development value | Shared ABP string-encryption passphrase |
| `LANDING_PORT` | `8080` | Landing page host port |
| `BLAZOR_WEBAPP_PORT` | `8081` | Blazor WebApp host port |
| `BLAZOR_SERVER_PORT` | `8082` | Blazor Server host port |
| `BLAZOR_WASM_PORT` | `8083` | Blazor WebAssembly host port |
| `ANGULAR_PORT` | `8084` | Angular host port |
| `MVC_PORT` | `8085` | MVC/Razor Pages host port |

If a public port is overridden, the entrypoint updates OpenIddict clients,
Angular runtime configuration, and landing-page links before startup. For
example:

```bash
BLAZOR_WEBAPP_PORT=18081 ANGULAR_PORT=18084 MVC_PORT=18085 docker compose up --build --detach --wait
```

The known development database password and encryption passphrase must never be
reused in a deployed environment.

### Resetting the demo

Stop without deleting data:

```bash
docker compose down
```

Delete the shared database volume and reseed a clean administrator account:

```bash
docker compose down --volumes
docker compose up --detach --wait
```

### Manual test checklist

- Open every URL and confirm the ABP Basic Theme, home page, localization menu,
  and FileClerk navigation/page render.
- Log in and out as `admin`, then open Identity users/roles and tenant
  administration in each UI.
- Call `/api/file-clerk/example` through ports 8083 and 8084 and confirm the
  response contains `{"value":42}`.
- Open `/FileClerk` and call `/api/file-clerk/example` through port 8085 to
  confirm the MVC module page and local API are available.
- Confirm Blazor WebApp server rendering becomes interactive and Blazor Server
  remains functional across navigation (WebSockets are proxied by Nginx).
- Create or change a user/tenant in one UI and confirm the shared state is
  visible from the other four.
- Restart the stack and confirm the change survives; use the reset procedure
  only when a clean database is wanted.

## Repository layout

| Path | Purpose |
| --- | --- |
| `src/src/` | Reusable ABP module projects and UI integrations |
| `src/test/` | Reusable module tests |
| `src/angular/` | Reusable Angular library workspace |
| `src/demo/shared/` | Shared layered backend, API host, EF migrations, and DbMigrator |
| `src/demo/angular/` | Angular demo application |
| `src/demo/blazor-server/` | Blazor Server demo |
| `src/demo/blazor-webapp/` | Blazor WebApp server and interactive client |
| `src/demo/blazor-webassembly/` | Blazor WebAssembly client and static host |
| `src/demo/mvc/` | MVC/Razor Pages demo |
| `src/demo/test/` | Shared backend Domain, Application, and EF Core tests |
| `src/Directory.Packages.props` | Centrally managed NuGet versions |
| `GitVersion.yml` | Release-line and semantic-version increment rules |
| `docker/` | Landing page and Linux application-server configuration |

## Contributing

Contributions are welcome. Read the
[contribution guide](.github/CONTRIBUTING.md), use the issue templates, and open
a pull request into `development`. Release promotion pull requests target
`main`. All changes must pass the required CI checks.

## Security

Do not report vulnerabilities in public issues. Follow the
[security policy](.github/SECURITY.md) to report them privately.

## License

FileClerk is licensed under the [MIT License](LICENSE).
