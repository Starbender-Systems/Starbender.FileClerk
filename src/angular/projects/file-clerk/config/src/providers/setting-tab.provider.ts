import { SettingTabsService } from '@abp/ng.setting-management/config';
import { provideAppInitializer, inject } from '@angular/core';
import { FileClerkSettingsComponent } from '@starbender/file-clerk';

export const FILE_CLERK_SETTING_TAB_PROVIDERS = [
  provideAppInitializer(() => {
    configureFileClerkSettingTab();
  }),
];

export function configureFileClerkSettingTab() {
  const settingTabs = inject(SettingTabsService);
  settingTabs.add([
    {
      name: 'FileClerk::FileClerk:FeatureManagement',
      order: 110,
      requiredPolicy: 'FileClerk.ManageFileClerk',
      component: FileClerkSettingsComponent,
    },
  ]);
}
