import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllUntzUsersComponent } from './all-untz-users.component';

describe('AllUntzUsersComponent', () => {
  let component: AllUntzUsersComponent;
  let fixture: ComponentFixture<AllUntzUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AllUntzUsersComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllUntzUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
