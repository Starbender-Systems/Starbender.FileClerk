import { inject, Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class FileClerkService {
  apiName = 'FileClerk';

  private restService = inject(RestService);

  sample() {
    return this.restService.request<void, any>(
      { method: 'GET', url: '/api/file-clerk/example' },
      { apiName: this.apiName }
    );
  }
}
