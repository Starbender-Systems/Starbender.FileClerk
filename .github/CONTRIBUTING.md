# Contributing to Starbender.FileClerk

Thank you for your interest in contributing to FileClerk. This repository uses
a fork-and-pull-request contribution model.

## Workflow

1. Search existing issues before opening a new report or request.
2. Fork the repository and create a focused branch from `development`.
3. Name issue branches `<issue-number>-<short-description>` when applicable.
4. Make the change and add or update tests and documentation.
5. Run the relevant local validation commands.
6. Open a pull request into `development` and link the related issue. Only
   release-promotion pull requests target `main`.
7. Resolve review conversations and wait for all required checks to pass.

## Pull request expectations

- Keep a pull request focused on one change or a tightly related set of changes.
- Explain what changed, why it is needed, and its user or developer impact.
- Update documentation when behavior, setup, dependencies, or public APIs change.
- Do not include secrets, private configuration, commercial package contents, or
  source that cannot be distributed under the repository license.
- Preserve the existing project structure and code style.

## Versioning and releases

All production projects are packaged at one shared version. Ordinary squash
merges advance the fix version. Add one of these directives to the squash
commit message when a different increment is intentional:

- `+semver: fix`
- `+semver: minor`
- `+semver: major`

Merges into `development` create prerelease NuGet artifacts but do not publish
them. Stable releases are promoted from `development` to `main`, then the exact
release commit is tagged `vMajor.Minor.Fix`. Only that validated tag workflow
can publish to NuGet.org.

## Local validation

### .NET

```bash
dotnet restore src/Starbender.FileClerk.slnx
dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore
dotnet test src/Starbender.FileClerk.slnx --configuration Release --no-build
```

### NuGet packages

```bash
dotnet tool restore
dotnet gitversion /showvariable MajorMinorPatch
dotnet pack src/Starbender.FileClerk.slnx --configuration Release \
  -p:Version=0.1.0-prerelease-local \
  -p:PackageVersion=0.1.0-prerelease-local \
  -p:PackageOutputPath=artifacts/nuget
./scripts/validate-nuget-packages.sh artifacts/nuget 0.1.0-prerelease-local
```

Use the current GitVersion value in place of `0.1.0` when the release line has
advanced. CI retains package artifacts for 30 days.

### Angular

```bash
cd src/angular
npm ci
npm run lint
npm run build
```

## Reporting issues

Use the repository issue templates for bugs and feature requests. Do not report
security vulnerabilities publicly; follow [`SECURITY.md`](SECURITY.md).

## Code of Conduct

Participation in this project is governed by
[`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md).
