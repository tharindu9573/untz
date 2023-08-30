import { Component } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TicketPurchased } from 'src/app/models/ticket-purchased';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';

@Component({
  selector: 'app-purchased-ticket-detailed',
  templateUrl: './purchased-ticket-detailed.component.html',
  styleUrls: ['./purchased-ticket-detailed.component.css']
})
export class PurchasedTicketDetailedComponent {
  public purchasedTicket!: TicketPurchased;

  constructor(private activeRoute: ActivatedRoute, private sanitizer: DomSanitizer, private ticketPurchaseService: TicketPurchaseService){}
  
  ngOnInit(): void {
    this.activeRoute.params.subscribe(_ => {
      if(_['referenceId']){
        let referenceId = _['referenceId'];
        this.ticketPurchaseService.getPurchasedTicketByReferenceDetailed(referenceId).subscribe(_ =>{
          this.purchasedTicket = JSON.parse(_);
        });
      }
    });
  }

  getRecipt(content?: string){    
    if(content){
      const pdfDataUrl = 'data:application/pdf;base64,' + content;
      return this.sanitizer.bypassSecurityTrustResourceUrl(pdfDataUrl);    
    }
    return undefined;
  }

  getImage(content?: string){    
    if(content){
      const pdfDataUrl = 'data:image/jpeg;base64,' + content;
      return this.sanitizer.bypassSecurityTrustResourceUrl(pdfDataUrl);    
    }
    return undefined;
  }
}
