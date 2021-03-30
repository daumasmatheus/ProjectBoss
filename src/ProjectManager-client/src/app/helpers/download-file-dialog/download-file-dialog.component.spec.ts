import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadFileDialogComponent } from './download-file-dialog.component';

describe('DownloadFileDialogComponent', () => {
  let component: DownloadFileDialogComponent;
  let fixture: ComponentFixture<DownloadFileDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DownloadFileDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadFileDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
