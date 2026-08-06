# Starbender.FileClerk

Starbender.FileClerk is an [ABP Framework](https://abp.io/) module for managing
BLOB storage containers and their provider configuration.

> [!IMPORTANT]
> FileClerk is under active development. No NuGet or npm packages have been
> released, and the current repository is a module foundation rather than a
> production-ready storage-management product.

## Project status

The repository currently provides the generated ABP module layers, Entity
Framework Core integration, HTTP API, MVC, Blazor, Angular, installer, and test
projects. Container management, provider registration, permissions, feature
configuration, and demo applications are tracked on the
[project issue list](https://github.com/Starbender-Systems/Starbender.FileClerk/issues).
Issue [#11](https://github.com/Starbender-Systems/Starbender.FileClerk/issues/11)
is the top-level feature roadmap.

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

### .NET solution

```bash
dotnet restore src/Starbender.FileClerk.slnx
dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore
dotnet test src/Starbender.FileClerk.slnx --configuration Release --no-build
```

### Angular library

Use a Node version manager that honors `.nvmrc`, then run:

```bash
cd src/angular
npm ci
npm run lint
npm run build
```

## Demo containers

The repository includes an immutable Docker Compose environment with two
services: PostgreSQL and a Linux application container. The application image
builds the complete .NET solution and Angular library, then serves a placeholder
landing page until the runnable demo applications are added.

Docker Engine with Docker Compose v2 is the only host prerequisite. Start the
environment from the repository root:

```bash
docker compose up --build --detach --wait
```

Open <http://localhost:8080> to view the landing page. Inspect container health
and logs with:

```bash
docker compose ps
docker compose logs --follow
```

The PostgreSQL service creates one database shared by every future demo host.
The application container receives both `ConnectionStrings__Default` and
`ConnectionStrings__FileClerk` with this internal connection information:

```text
Host=postgres;Port=5432;Database=fileclerk;Username=fileclerk;Password=fileclerk_dev_only
```

> [!WARNING]
> `fileclerk_dev_only` is a known local-development password. Never use it in a
> deployed environment.

All settings have Compose defaults and can be overridden with shell environment
variables or an ignored `.env` file. Copy `.env.example` when a persistent local
override file is useful.

| Variable | Default | Purpose |
| --- | --- | --- |
| `POSTGRES_DB` | `fileclerk` | Shared database name |
| `POSTGRES_USER` | `fileclerk` | Database user |
| `POSTGRES_PASSWORD` | `fileclerk_dev_only` | Local-only database password |
| `DATABASE_BIND_ADDRESS` | `127.0.0.1` | PostgreSQL host bind address |
| `POSTGRES_PORT` | `5432` | PostgreSQL host port |
| `APP_BIND_ADDRESS` | `127.0.0.1` | Demo application host bind address |
| `LANDING_PORT` | `8080` | Landing page host port |
| `MVC_PORT` | `8081` | Reserved MVC/Razor Pages host port |
| `BLAZOR_SERVER_PORT` | `8082` | Reserved Blazor Server host port |
| `BLAZOR_WASM_PORT` | `8083` | Reserved Blazor WebAssembly host port |
| `ANGULAR_PORT` | `8084` | Reserved Angular host port |

Only the landing page listens today; the other ports reserve a stable contract
for the upcoming demos. Angular and Blazor WebAssembly will serve static content
and reverse-proxy their backend routes through their single assigned ports.

Stop the environment while retaining the database volume:

```bash
docker compose down
```

To intentionally delete all demo database data and start fresh, remove the named
volume as well:

```bash
docker compose down --volumes
```

The module remains database-provider-neutral. The future demo hosts will own the
PostgreSQL provider configuration and migrations. One designated migrator must
finish before Supervisor starts any host; individual UI applications must not
run competing migrations.

## Repository layout

| Path | Purpose |
| --- | --- |
| `src/src/` | Reusable ABP module projects and UI integrations |
| `src/test/` | Shared, domain, application, and EF Core tests |
| `src/angular/` | Angular library workspace |
| `src/Directory.Packages.props` | Centrally managed NuGet versions |
| `docker/` | Landing page and Linux application-server configuration |

## Contributing

Contributions are welcome. Read the
[contribution guide](.github/CONTRIBUTING.md), use the issue templates, and open
a pull request into `main`. All changes must pass the required CI checks.

## Security

Do not report vulnerabilities in public issues. Follow the
[security policy](.github/SECURITY.md) to report them privately.

## License

FileClerk is licensed under the [MIT License](LICENSE).
