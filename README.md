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
| .NET SDK | 10.0.100 or a later 10.0 patch |
| ABP Framework | 10.5.0 |
| Angular | 21.2.x |
| Node.js | 22.13.0 |

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

## Repository layout

| Path | Purpose |
| --- | --- |
| `src/src/` | Reusable ABP module projects and UI integrations |
| `src/test/` | Shared, domain, application, and EF Core tests |
| `src/angular/` | Angular library workspace |
| `src/Directory.Packages.props` | Centrally managed NuGet versions |

## Contributing

Contributions are welcome. Read the
[contribution guide](.github/CONTRIBUTING.md), use the issue templates, and open
a pull request into `main`. All changes must pass the required CI checks.

## Security

Do not report vulnerabilities in public issues. Follow the
[security policy](.github/SECURITY.md) to report them privately.

## License

FileClerk is licensed under the [MIT License](LICENSE).
