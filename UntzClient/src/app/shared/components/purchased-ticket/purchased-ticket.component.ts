import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TicketPurchased } from 'src/app/models/ticket-purchased';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';

@Component({
  selector: 'app-purchased-ticket',
  templateUrl: './purchased-ticket.component.html',
  styleUrls: ['./purchased-ticket.component.css']
})
export class PurchasedTicketComponent implements OnInit{
  
  public purchasedTicket!: TicketPurchased;

  constructor(private activeRoute: ActivatedRoute, private sanitizer: DomSanitizer, private ticketPurchaseService: TicketPurchaseService){}
  
  ngOnInit(): void {
    this.activeRoute.params.subscribe(_ => {
      if(_['referenceId']){
        let referenceId = _['referenceId'];
        this.ticketPurchaseService.getPurchasedTicketByReference(referenceId).subscribe(_ =>{
          this.purchasedTicket = JSON.parse(_);
        });
      }
    });
  }
}
