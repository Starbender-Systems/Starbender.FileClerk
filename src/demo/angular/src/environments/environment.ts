import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:8084';

const oAuthConfig = {
  issuer: `${baseUrl}/`,
  redirectUri: baseUrl,
  clientId: 'FileClerkDemo_Angular',
  responseType: 'code',
  scope: 'offline_access Demo',
  requireHttps: false,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'FileClerk Angular Demo',
  },
  oAuthConfig,
  apis: {
    default: {
      url: baseUrl,
      rootNamespace: 'Starbender.FileClerk.Demo',
    },
    FileClerk: {
      url: baseUrl,
      rootNamespace: 'Starbender.FileClerk',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
