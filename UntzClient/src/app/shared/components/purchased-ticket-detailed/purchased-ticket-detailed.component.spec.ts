import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchasedTicketDetailedComponent } from './purchased-ticket-detailed.component';

describe('PurchasedTicketDetailedComponent', () => {
  let component: PurchasedTicketDetailedComponent;
  let fixture: ComponentFixture<PurchasedTicketDetailedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PurchasedTicketDetailedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurchasedTicketDetailedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
