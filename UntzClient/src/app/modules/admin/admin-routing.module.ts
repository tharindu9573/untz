import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventComponent } from './event/event.component';
import { AllUntzUsersComponent } from './all-untz-users/all-untz-users.component';
import { AllGuestUsersComponent } from './all-guest-users/all-guest-users.component';

const routes: Routes = [
  {
    path: 'event',
    component: EventComponent
  },
  {
    path: 'all-untz-users',
    component: AllUntzUsersComponent
  },
  {
    path: 'all-guest-users',
    component: AllGuestUsersComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
