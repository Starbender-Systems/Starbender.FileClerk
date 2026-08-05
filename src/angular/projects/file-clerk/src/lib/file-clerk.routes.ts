import { RouterOutletComponent } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const FILE_CLERK_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: RouterOutletComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./components/file-clerk.component').then(c => c.FileClerkComponent),
      },
    ],
  },
];
