import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { FileClerkService } from '../services/file-clerk.service';
import { FileClerkComponent } from './file-clerk.component';

describe('FileClerkComponent', () => {
  it('renders the library smoke-test content', async () => {
    await TestBed.configureTestingModule({
      imports: [FileClerkComponent],
      providers: [
        {
          provide: FileClerkService,
          useValue: { sample: () => of(undefined) },
        },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(FileClerkComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('p')?.textContent).toContain(
      'file-clerk works!'
    );
  });
});
