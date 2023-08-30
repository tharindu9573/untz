import { Component } from '@angular/core';
import { GuestUser } from 'src/app/models/guest-user';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';
import { UserService } from 'src/services/user.service';
import { UserTicketsComponent } from '../user-tickets/user-tickets.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserConfirmationPopupComponent } from '../user-confirmation-popup/user-confirmation-popup.component';

@Component({
  selector: 'app-all-guest-users',
  templateUrl: './all-guest-users.component.html',
  styleUrls: ['./all-guest-users.component.css']
})
export class AllGuestUsersComponent {
  public guestUsers: GuestUser[] = [];

  constructor(private userService: UserService, private ticketPurchaseService: TicketPurchaseService, private modelService: NgbModal){
  }
  ngOnInit(): void {
    this.userService.getGuestUsers().subscribe(_ => {
      this.guestUsers = _;
    })
  }

  viewTicketsForGuestUser(user: GuestUser){
    this.ticketPurchaseService.getPurchasedTicketsForGuestUser(user.id).subscribe(_ => {
      var model = this.modelService.open(UserTicketsComponent, { size: 'xl', backdrop: 'static' });
      model.componentInstance.purchasedTickets = JSON.parse(_);
      model.componentInstance.userName = `${user.firstName} ${user.lastName}`;
      model.componentInstance.closePopup.subscribe(() => {
        model.close();
      })
    });
  }

  deleteUser(id: number){
    var confirmationModel = this.modelService.open(UserConfirmationPopupComponent, { backdrop: 'static' })
    confirmationModel.componentInstance.content = `Do you want to delete this user ? if you proceed, all the tickets associated to this user will be deleted`;
    confirmationModel.componentInstance.command.subscribe((_: boolean) => {
      if(_){
        this.userService.deleteGuestUser(id).subscribe(_ => {
          if(_ === true){
           this.guestUsers = this.guestUsers.filter(_ => _.id !== id);            
          }
        })
      }
      confirmationModel.close();      
    })
  }
}
