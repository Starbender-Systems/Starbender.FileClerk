import { inject, Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

export interface FileClerkProviderChoice {
  id: number;
  name: string;
}

export interface FileClerkConfiguration {
  effectiveEnabled: boolean;
  selectedProviderIds: number[];
  availableProviders: FileClerkProviderChoice[];
  canManageProviderRegistry: boolean;
}

export interface UpdateFileClerkConfiguration {
  enabled: boolean;
  providerIds: number[];
}

export interface FileClerkBlobProvider {
  id: number;
  name: string;
  implementationType: string;
  configurationSchema: string;
  enabled: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class FileClerkService {
  apiName = 'FileClerk';

  private readonly restService = inject(RestService);

  sample() {
    return this.restService.request<void, unknown>(
      { method: 'GET', url: '/api/file-clerk/example' },
      { apiName: this.apiName }
    );
  }

  getConfiguration() {
    return this.restService.request<void, FileClerkConfiguration>(
      { method: 'GET', url: '/api/file-clerk/configuration' },
      { apiName: this.apiName }
    );
  }

  updateConfiguration(input: UpdateFileClerkConfiguration) {
    return this.restService.request<UpdateFileClerkConfiguration, FileClerkConfiguration>(
      {
        method: 'PUT',
        url: '/api/file-clerk/configuration',
        body: input,
      },
      { apiName: this.apiName }
    );
  }

  getProviders() {
    return this.restService.request<void, FileClerkBlobProvider[]>(
      { method: 'GET', url: '/api/file-clerk/blob-providers' },
      { apiName: this.apiName }
    );
  }

  setProviderEnabled(id: number, enabled: boolean) {
    return this.restService.request<{ enabled: boolean }, FileClerkBlobProvider>(
      {
        method: 'PUT',
        url: `/api/file-clerk/blob-providers/${id}/enabled`,
        body: { enabled },
      },
      { apiName: this.apiName }
    );
  }
}
