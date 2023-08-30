import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { EventComponent } from './event/event.component';
import { AllUntzUsersComponent } from './all-untz-users/all-untz-users.component';
import { AllGuestUsersComponent } from './all-guest-users/all-guest-users.component';
import { EventFormComponent } from './event/event-form/event-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EventConfirmationPopupComponent } from './event/event-confirmation-popup/event-confirmation-popup.component';
import { UserTicketsComponent } from './user-tickets/user-tickets.component';
import { ImageViewerComponent } from './image-viewer/image-viewer.component';
import { UserConfirmationPopupComponent } from './user-confirmation-popup/user-confirmation-popup.component';

@NgModule({
  declarations: [
    EventComponent,
    AllUntzUsersComponent,
    AllGuestUsersComponent,
    EventFormComponent,
    EventConfirmationPopupComponent,
    UserTicketsComponent,
    ImageViewerComponent,
    UserConfirmationPopupComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class AdminModule { }
