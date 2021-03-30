import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectAttendantListComponent } from './select-attendant-list.component';

describe('SelectAttendantListComponent', () => {
  let component: SelectAttendantListComponent;
  let fixture: ComponentFixture<SelectAttendantListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectAttendantListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectAttendantListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
