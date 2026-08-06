# FileClerk - Angular Library

This is an Angular library for the ABP module. It provides UI components and services that can be used in Angular applications. For more information, visit [abp.io](https://abp.io/).

## Pre-requirements

* [Node.js 22.13.0](https://nodejs.org/), as pinned by the repository `.nvmrc`
* [npm](https://www.npmjs.com/)

## Getting Started

### Install dependencies

```bash
npm ci
```

## Development

### Build the library

```bash
ng build file-clerk
```

The build artifacts will be stored in the `dist/` directory.

### Watch mode

For development with automatic rebuilds:

```bash
ng build file-clerk --watch
```

## Code scaffolding

Run `ng generate component component-name --project file-clerk` to generate a new component in the library.

## Running unit tests

Run `npm test -- --watch=false` to execute the unit tests with Vitest.

## Publishing

After building the library, you can publish it to npm:

```bash
cd dist/file-clerk
npm publish
```

## Using in a Host Application

To use this library in an ABP Angular application:

1. Install the package:
```bash
npm install @starbender/file-clerk
```

2. Import the module in your application:
```typescript
import { FileClerkModule } from '@starbender/file-clerk';

@NgModule({
  imports: [
    // ...
    FileClerkModule
  ]
})
export class AppModule { }
```

## Additional Resources

* [ABP Angular UI Documentation](https://abp.io/docs/latest/framework/ui/angular/overview)
* [Angular Library Development](https://angular.dev/tools/libraries)
* [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli)
