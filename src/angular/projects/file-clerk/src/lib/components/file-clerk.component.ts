import { Component, inject } from '@angular/core';
import { FileClerkService } from '../services/file-clerk.service';

@Component({
  selector: 'lib-file-clerk',
  template: ` <p>file-clerk works!</p> `,
})
export class FileClerkComponent {
  protected readonly service = inject(FileClerkService);

  constructor() {
    this.service.sample().subscribe(console.log);
  }
}
