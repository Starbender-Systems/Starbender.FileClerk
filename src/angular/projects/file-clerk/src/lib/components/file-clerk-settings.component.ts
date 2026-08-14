import { LocalizationPipe } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FileClerkBlobProvider,
  FileClerkConfiguration,
  FileClerkService,
} from '../services/file-clerk.service';

@Component({
  selector: 'lib-file-clerk-settings',
  standalone: true,
  imports: [CommonModule, LocalizationPipe],
  template: `
    @if (!configuration) {
      <p>{{ 'FileClerk::FileClerk:Loading' | abpLocalization }}</p>
    } @else {
      <div class="py-2">
        <div class="form-check mb-3">
          <input
            id="file-clerk-enabled-angular"
            class="form-check-input"
            type="checkbox"
            [checked]="enabled"
            (change)="enabled = $any($event.target).checked"
          />
          <label class="form-check-label" for="file-clerk-enabled-angular">
            {{ 'FileClerk::FileClerk:Enabled' | abpLocalization }}
          </label>
          <div class="form-text">
            {{ 'FileClerk::FileClerk:FileClerkEnabledDescription' | abpLocalization }}
          </div>
        </div>

        <fieldset class="mb-3">
          <legend class="h6">{{ 'FileClerk::FileClerk:Providers' | abpLocalization }}</legend>
          <div class="form-text mb-2">
            {{ 'FileClerk::FileClerk:FileClerkProvidersDescription' | abpLocalization }}
          </div>
          @for (provider of configuration.availableProviders; track provider.id) {
            <div class="form-check">
              <input
                class="form-check-input"
                type="checkbox"
                [id]="'file-clerk-provider-angular-' + provider.id"
                [checked]="selectedProviderIds.has(provider.id)"
                (change)="toggleProvider(provider.id, $any($event.target).checked)"
              />
              <label
                class="form-check-label"
                [for]="'file-clerk-provider-angular-' + provider.id"
              >
                {{ provider.name }}
              </label>
            </div>
          }
        </fieldset>

        <button type="button" class="btn btn-primary" (click)="save()">
          <i class="fa fa-save me-1"></i>
          {{ 'FileClerk::FileClerk:Save' | abpLocalization }}
        </button>

        @if (configuration.canManageProviderRegistry) {
          <hr />
          <h5>{{ 'FileClerk::FileClerk:ProviderRegistry' | abpLocalization }}</h5>
          <div class="table-responsive">
            <table class="table table-striped align-middle">
              <thead>
                <tr>
                  <th>{{ 'FileClerk::FileClerk:Providers' | abpLocalization }}</th>
                  <th>{{ 'FileClerk::FileClerk:ImplementationType' | abpLocalization }}</th>
                  <th>{{ 'FileClerk::FileClerk:Enabled' | abpLocalization }}</th>
                </tr>
              </thead>
              <tbody>
                @for (provider of registry; track provider.id) {
                  <tr>
                    <td>{{ provider.name }}</td>
                    <td><code>{{ provider.implementationType }}</code></td>
                    <td>
                      <input
                        class="form-check-input"
                        type="checkbox"
                        [checked]="provider.enabled"
                        (change)="setRegistryEnabled(provider, $any($event.target).checked)"
                      />
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    }
  `,
})
export class FileClerkSettingsComponent implements OnInit {
  private readonly service = inject(FileClerkService);

  configuration?: FileClerkConfiguration;
  registry: FileClerkBlobProvider[] = [];
  selectedProviderIds = new Set<number>();
  enabled = false;

  ngOnInit() {
    this.load();
  }

  toggleProvider(id: number, selected: boolean) {
    if (selected) {
      this.selectedProviderIds.add(id);
    } else {
      this.selectedProviderIds.delete(id);
    }
  }

  save() {
    this.service
      .updateConfiguration({
        enabled: this.enabled,
        providerIds: Array.from(this.selectedProviderIds).sort((left, right) => left - right),
      })
      .subscribe(configuration => this.applyConfiguration(configuration));
  }

  setRegistryEnabled(provider: FileClerkBlobProvider, enabled: boolean) {
    this.service.setProviderEnabled(provider.id, enabled).subscribe(() => this.load());
  }

  private load() {
    this.service.getConfiguration().subscribe(configuration => {
      this.applyConfiguration(configuration);
      if (configuration.canManageProviderRegistry) {
        this.service.getProviders().subscribe(providers => (this.registry = providers));
      }
    });
  }

  private applyConfiguration(configuration: FileClerkConfiguration) {
    this.configuration = configuration;
    this.enabled = configuration.effectiveEnabled;
    this.selectedProviderIds = new Set(configuration.selectedProviderIds);
  }
}
