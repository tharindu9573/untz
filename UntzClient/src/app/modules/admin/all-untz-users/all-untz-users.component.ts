import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthUser } from 'src/app/models/auth-user';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';
import { UserService } from 'src/services/user.service';
import { UserTicketsComponent } from '../user-tickets/user-tickets.component';
import { UserConfirmationPopupComponent } from '../user-confirmation-popup/user-confirmation-popup.component';
import { RegisterComponent } from '../../auth/register/register.component';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-all-untz-users',
  templateUrl: './all-untz-users.component.html',
  styleUrls: ['./all-untz-users.component.css']
})
export class AllUntzUsersComponent implements OnInit {

  public authUsers: AuthUser[] = [];

  constructor(private userService: UserService, private authService: AuthService, private ticketPurchaseService: TicketPurchaseService, private modelService: NgbModal){
  }
  ngOnInit(): void {
    this.userService.getAllUntzUsers().subscribe(_ => {
      this.authUsers = _;
    })
  }

  viewTicketsForUser(user: AuthUser){
    this.ticketPurchaseService.getPurchasedTicketsForUntzUser(user.id).subscribe(_ => {
      var model = this.modelService.open(UserTicketsComponent, { size: 'xl', backdrop: 'static' });
      model.componentInstance.purchasedTickets = JSON.parse(_);
      model.componentInstance.userName = `${user.firstName} ${user.lastName}`;
      model.componentInstance.closePopup.subscribe(() => {
        model.close();
      })
    });
  }

  deleteUser(id: string){
    var confirmationModel = this.modelService.open(UserConfirmationPopupComponent, { backdrop: 'static' })
    confirmationModel.componentInstance.content = `Do you want to delete this user ? if you proceed, all the tickets associated to this user will be deleted`;
    confirmationModel.componentInstance.command.subscribe((_: boolean) => {
      if(_){
        this.userService.deleteUntzUser(id).subscribe(_ => {
          if(_ === true){
           this.authUsers = this.authUsers.filter(_ => _.id !== id);            
          }
        })
      }
      confirmationModel.close();      
    })
  }

  addUser(){
    this.authService.getAllRoles().subscribe(_ => {
      var model = this.modelService.open(RegisterComponent, { size: 'xl', backdrop: 'static' });
      model.componentInstance.isCreatedByAdmin = true;
      model.componentInstance.roles = _;
      model.componentInstance.closePopup.subscribe((_: AuthUser) => {
        if(_ !== undefined){
          this.authUsers.push(_);          
        }
        model.close();
      })
    })
  }
}
