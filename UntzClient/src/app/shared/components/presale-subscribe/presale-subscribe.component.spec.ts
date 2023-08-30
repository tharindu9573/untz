import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PresaleSubscribeComponent } from './presale-subscribe.component';

describe('PresaleSubscribeComponent', () => {
  let component: PresaleSubscribeComponent;
  let fixture: ComponentFixture<PresaleSubscribeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PresaleSubscribeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PresaleSubscribeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
