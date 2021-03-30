import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectAttendantDialogComponent } from './select-attendant-dialog.component';

describe('SelectAttendantDialogComponent', () => {
  let component: SelectAttendantDialogComponent;
  let fixture: ComponentFixture<SelectAttendantDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectAttendantDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectAttendantDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
