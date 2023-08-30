import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthUser } from 'src/app/models/auth-user';
import { UntzEvent } from 'src/app/models/event';
import { AuthService } from 'src/services/auth.service';
import { EventService } from 'src/services/event.service';

@Component({
  selector: 'app-presale-subscribe',
  templateUrl: './presale-subscribe.component.html',
  styleUrls: ['./presale-subscribe.component.css']
})
export class PresaleSubscribeComponent implements OnInit{
  public event!: UntzEvent;
  public isAuthenticated: boolean = false;
  public user!: AuthUser | undefined;
  public guestUserMobileNumber!: string;
  public guestUserEmail!: string;
  public isSubscribeButtonIsDisabled: boolean = false;

  @Output() closePopup = new EventEmitter();

  constructor(private authService: AuthService, 
    private eventService: EventService, private toastrService: ToastrService,
    private router: Router){

  }
  ngOnInit(): void {
    this.authService.isAuthenticated.subscribe(_ => {
      this.isAuthenticated = _;
      this.user = this.authService.authUser;
    });
  }

  cancel(){
    this.closePopup.emit();
  }

  subscribe(){    
        
    let body = this.getSubscribeBody();

    if(body.email === undefined || body.email === undefined){
      this.toastrService.warning("Please fill all required inputs.");
      return;
    }
    else
    {
      this.isSubscribeButtonIsDisabled = true;
      this.eventService.subscribe(body).subscribe(_ => {
        if(_ === true){
          this.toastrService.success("Subscribed to the event");
          this.isSubscribeButtonIsDisabled = false;
          this.cancel();
          this.router.navigate(["/"]);
        }
      })
    }
  }

  getSubscribeBody(){
    return {
      eventId: this.event.id,
      email: this.isAuthenticated ? this.user?.email : this.guestUserEmail,
      phoneNumber: this.isAuthenticated ? this.user?.phoneNumber : this.guestUserMobileNumber
    }
  };
}
