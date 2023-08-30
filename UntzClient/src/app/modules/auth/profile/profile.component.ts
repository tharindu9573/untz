import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthUser } from 'src/app/models/auth-user';
import { TicketPurchased } from 'src/app/models/ticket-purchased';
import { AuthService } from 'src/services/auth.service';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  public purchasedTickets!: TicketPurchased[];
  public untzUser!: AuthUser;

  constructor(private authService: AuthService, private tickerPurchaseService: TicketPurchaseService, private router: Router, private modelService: NgbModal){

  }
  ngOnInit(): void {

    this.authService.isAuthenticated.subscribe((_) =>{
      if(_){
        this.untzUser = this.authService.authUser!;
      }
    });

    this.tickerPurchaseService.getPurchasedTicketsForCurrentUser().subscribe(_ => {
      this.purchasedTickets = JSON.parse(_);
    })
  }

  getRole(user: AuthUser): string{
    return user.roles.includes('Admin') ? 'Admin' : 'User';
  }

  navigateToEvent(referenceId: number){
    this.router.navigate([`ticketpurchased/${referenceId}/detailed`]);
  }

  editInfo(){
    var model = this.modelService.open(RegisterComponent, { size: 'lg', backdrop: 'static' });
    model.componentInstance.editMode = true;
    model.componentInstance.user = this.untzUser;
    model.componentInstance.closePopup.subscribe((_: AuthUser | undefined) => {
      if(_){
        this.untzUser = _;        
      }
      model.close();
    })
  }

}
