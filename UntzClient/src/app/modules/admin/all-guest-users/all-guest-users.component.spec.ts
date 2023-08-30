import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllGuestUsersComponent } from './all-guest-users.component';

describe('AllGuestUsersComponent', () => {
  let component: AllGuestUsersComponent;
  let fixture: ComponentFixture<AllGuestUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AllGuestUsersComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllGuestUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
