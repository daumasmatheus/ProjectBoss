import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PopConfirmationComponent } from './pop-confirmation.component';

describe('PopConfirmationComponent', () => {
  let component: PopConfirmationComponent;
  let fixture: ComponentFixture<PopConfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PopConfirmationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PopConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
