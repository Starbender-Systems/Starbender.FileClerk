# Contributing to Starbender.FileClerk

Thank you for your interest in contributing to FileClerk. This repository uses
a fork-and-pull-request contribution model.

## Workflow

1. Search existing issues before opening a new report or request.
2. Fork the repository and create a focused branch from `main`.
3. Name issue branches `<issue-number>-<short-description>` when applicable.
4. Make the change and add or update tests and documentation.
5. Run the relevant local validation commands.
6. Open a pull request into `main` and link the related issue.
7. Resolve review conversations and wait for all required checks to pass.

## Pull request expectations

- Keep a pull request focused on one change or a tightly related set of changes.
- Explain what changed, why it is needed, and its user or developer impact.
- Update documentation when behavior, setup, dependencies, or public APIs change.
- Do not include secrets, private configuration, commercial package contents, or
  source that cannot be distributed under the repository license.
- Preserve the existing project structure and code style.

## Local validation

### .NET

```bash
dotnet restore src/Starbender.FileClerk.slnx
dotnet build src/Starbender.FileClerk.slnx --configuration Release --no-restore
dotnet test src/Starbender.FileClerk.slnx --configuration Release --no-build
```

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
