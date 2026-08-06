const tseslint = require('typescript-eslint');
const angular = require('angular-eslint');

module.exports = tseslint.config(
  {
    files: ['projects/file-clerk/**/*.ts'],
    extends: [...angular.configs.tsRecommended],
    processor: angular.processInlineTemplates,
    languageOptions: {
      parserOptions: {
        project: [
          'projects/file-clerk/tsconfig.lib.json',
          'projects/file-clerk/tsconfig.spec.json',
        ],
        tsconfigRootDir: __dirname,
      },
    },
    rules: {
      '@angular-eslint/directive-selector': [
        'error',
        { type: 'attribute', prefix: 'lib', style: 'camelCase' },
      ],
      '@angular-eslint/component-selector': [
        'error',
        { type: 'element', prefix: 'lib', style: 'kebab-case' },
      ],
    },
  },
  {
    files: ['projects/file-clerk/**/*.html'],
    extends: [...angular.configs.templateRecommended],
  },
);
