import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AuthService } from 'src/services/auth.service';
import { TicketService } from 'src/services/ticket.service';
import { Ticket } from 'src/app/models/ticket';
import { AuthUser } from 'src/app/models/auth-user';
import { PaymentMethod } from 'src/app/models/payment-method';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from 'src/services/user.service';
import { GuestUser } from 'src/app/models/guest-user';
import { UntzEvent } from 'src/app/models/event';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ticket-purchase',
  templateUrl: './ticket-purchase.component.html',
  styleUrls: ['./ticket-purchase.component.css']
})
export class TicketPurchaseComponent implements OnInit {

  public event!: UntzEvent;
  public tickets!: Ticket[];
  public paymentMethods!: PaymentMethod[];
  public selectedTicket: number = 0;
  public selectedTicketCount: number = 0;
  public selectedPaymentMethod: number = 0;
  public isAuthenticated: boolean = false;
  public user!: AuthUser | undefined;
  public total: number = 0;
  public ticketCountList: number[] = [];
  public guestUserFirstName!: string;
  public guestUserLastName!: string;
  public guestUserMobileNumber!: string;
  public guestUserEmail!: string;
  public isPurchasedButtonIsDisabled: boolean = false;

  @Output() closePopup = new EventEmitter();

  constructor(private authService: AuthService, private ticketService: TicketService, 
    private ticketPurchaseService: TicketPurchaseService, private toastrService: ToastrService, private userService: UserService,
    private router: Router){

  }
  ngOnInit(): void {
    this.ticketService.getTicketsForEvent(this.event.id).subscribe(_ => {
      this.tickets = _;
    });

    this.authService.isAuthenticated.subscribe(_ => {
      this.isAuthenticated = _;
      this.user = this.authService.authUser;
    });

    this.ticketService.getPaymentMethods().subscribe(_ => {
      this.paymentMethods = _;
    });

    for(let i = 1; i <= 10; i++){
      this.ticketCountList.push(i);
    }
  }

  cancel(){
    this.closePopup.emit();
  }

  getPurchasedBody(){
    return {
      ticketId: this.selectedTicket,
      noOfTickets: this.selectedTicketCount,
      paymentMethodId: this.selectedPaymentMethod,
      guestUserId: 0
    }
  };

  purchase(){    
    var body = this.getPurchasedBody();
    if(!this.validate()){
      this.toastrService.warning("Please fill all required inputs.");
      return;
    };
    
    if(this.user){
      this.isPurchasedButtonIsDisabled = true;
      this.ticketPurchaseService.GenerateBodyWithHash(body).subscribe(_ => {
        if(_ && _.custom_2 !== null || _.custom_2 !== undefined){
          this.navigateToPaymentGateway(_);
        }
      });
    }
    else{
      if(this.guestUserFirstName === undefined || this.guestUserLastName === undefined || this.guestUserMobileNumber === undefined || this.guestUserMobileNumber === undefined){
        this.toastrService.warning("Please fill all required inputs.");
      }
      else
      {
        this.isPurchasedButtonIsDisabled = true;
        this.userService.createGuestUser({
          firstName: this.guestUserFirstName,
          lastName: this.guestUserLastName,
          email: this.guestUserEmail,
          phoneNumber: this.guestUserMobileNumber
        } as GuestUser).subscribe(_ => {
          if(_.id > 0){
              body.guestUserId = _.id;
              this.ticketPurchaseService.GenerateBodyWithHash(body).subscribe(_ => {
                if(_ && _.custom_2 !== null || _.custom_2 !== undefined){
                  this.navigateToPaymentGateway(_);
                }
            });
          }else{
            this.toastrService.error("An error occured while processing your request!. Please try again later");
            this.isPurchasedButtonIsDisabled = false;
          }
        })
      }
    }
  }

  navigateToPaymentGateway(body: any){
    const form = document.createElement('form');
    form.method = 'post';
    form.action = 'https://sandbox.payhere.lk/pay/checkout';
     

    for(let key in body){
      let input = document.createElement('input');
      input.type = 'hidden';
      input.name = key;
      input.value = body[key];
      form.appendChild(input);
    }
    console.log(form);
    document.body.appendChild(form);
    form.submit();
  }

  ticketChanged(){
    this.calculateTotal();
  }

  ticketCountChanged(){
    this.calculateTotal();
  }

  calculateTotal(){
    let ticketPrice = this.tickets.find(_ => _.id === this.selectedTicket)?.price;
    if(ticketPrice){
      this.total = ticketPrice * this.selectedTicketCount;
    }
  }

  validate(): boolean{
    if(this.selectedPaymentMethod === 0 || this.selectedTicket === 0 || this.selectedTicketCount === 0){
      return false;
    }

    return true;
  }
}
