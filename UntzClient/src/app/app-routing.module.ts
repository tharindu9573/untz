import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './shared/components/home/home.component';
import { EventComponent } from './shared/components/event/event.component';
import { EventViewComponent } from './shared/components/event-view/event-view.component';
import { PurchasedTicketComponent } from './shared/components/purchased-ticket/purchased-ticket.component';
import { AboutComponent } from './shared/components/about/about.component';
import { PurchasedTicketDetailedComponent } from './shared/components/purchased-ticket-detailed/purchased-ticket-detailed.component';
import { TicketViewComponent } from './shared/components/ticket-view/ticket-view.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'about',
    component: AboutComponent
  },
  {
    path: 'event',
    component: EventComponent
  },
  {
    path: 'ticketpurchased/:referenceId/detailed',
    component: PurchasedTicketDetailedComponent
  },
  {
    path: 'ticketpurchased/:referenceId',
    component: PurchasedTicketComponent
  },
  {
    path: 'ticketview/:purchasedReferenceId/:ticketReferenceId',
    component: TicketViewComponent
  },
  {
    path: 'event/:id',
    component: EventViewComponent
  },
  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/auth-routing.module').then(_ => _.AuthRoutingModule)
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin/admin-routing.module').then(_ => _.AdminRoutingModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
