import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventConfirmationPopupComponent } from './event-confirmation-popup.component';

describe('EventConfirmationPopupComponent', () => {
  let component: EventConfirmationPopupComponent;
  let fixture: ComponentFixture<EventConfirmationPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EventConfirmationPopupComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventConfirmationPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
