import { eLayoutType, RoutesService } from '@abp/ng.core';
import {
  EnvironmentProviders,
  inject,
  makeEnvironmentProviders,
  provideAppInitializer,
} from '@angular/core';
import { eFileClerkRouteNames } from '../enums/route-names';
import { FILE_CLERK_SETTING_TAB_PROVIDERS } from './setting-tab.provider';

export const FILE_CLERK_ROUTE_PROVIDERS = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

export function configureRoutes() {
  const routesService = inject(RoutesService);
  routesService.add([
    {
      path: '/file-clerk',
      name: eFileClerkRouteNames.FileClerk,
      iconClass: 'fas fa-book',
      layout: eLayoutType.application,
      order: 3,
    },
  ]);
}

const FILE_CLERK_PROVIDERS: EnvironmentProviders[] = [
  ...FILE_CLERK_ROUTE_PROVIDERS,
  ...FILE_CLERK_SETTING_TAB_PROVIDERS,
];

export function provideFileClerk() {
  return makeEnvironmentProviders(FILE_CLERK_PROVIDERS);
}
