import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TicketPurchased } from 'src/app/models/ticket-purchased';
import { AuthService } from 'src/services/auth.service';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';

@Component({
  selector: 'app-ticket-view',
  templateUrl: './ticket-view.component.html',
  styleUrls: ['./ticket-view.component.css']
})
export class TicketViewComponent implements OnInit{

  public purchasedTicket?: TicketPurchased;
  public isInValidRole: boolean = false;
  public isAdmitButtonDisabled = false; 
  
  constructor(private authService: AuthService, private activeRoute: ActivatedRoute, 
    private sanitizer: DomSanitizer, private ticketPurchaseService: TicketPurchaseService,
    private toastrService: ToastrService){}
  ngOnInit(): void {
    this.activeRoute.params.subscribe(_ => {
      if(_['purchasedReferenceId'] && _['ticketReferenceId']){
        let purchasedReferenceId = _['purchasedReferenceId'];
        let ticketReferenceId = _['ticketReferenceId'];
        this.ticketPurchaseService.getPurchasedTicketAloneByReference(purchasedReferenceId, ticketReferenceId).subscribe(_ =>{
          this.purchasedTicket = JSON.parse(_);          
        });
      }
    });

    this.authService.isAuthenticated.subscribe(_ => {
      if(_ && (this.authService.authUser?.roles.includes("Admin") || this.authService.authUser?.roles.includes("Sales"))){
        this.isInValidRole = true;
      }else{
        this.isInValidRole = false;
      }
    })
  }

  getImage(content?: string){    
    if(content){
      const pdfDataUrl = 'data:image/jpeg;base64,' + content;
      return this.sanitizer.bypassSecurityTrustResourceUrl(pdfDataUrl);    
    }
    return undefined;
  }

  admit(referenceId: number){
    this.isAdmitButtonDisabled = true;
    this.ticketPurchaseService.admit(referenceId).subscribe(_ =>{
      if(_ === true){
        this.toastrService.success("Admitted!");
        this.purchasedTicket!.isAdmitted = true;
      }
      this.isAdmitButtonDisabled = false;
    })
  }
}
