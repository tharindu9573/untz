import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketPurchased } from 'src/app/models/ticket-purchased';
import { ImageViewerComponent } from '../image-viewer/image-viewer.component';
import { DomSanitizer } from '@angular/platform-browser';
import { UserConfirmationPopupComponent } from '../user-confirmation-popup/user-confirmation-popup.component';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';

@Component({
  selector: 'app-user-tickets',
  templateUrl: './user-tickets.component.html',
  styleUrls: ['./user-tickets.component.css']
})
export class UserTicketsComponent implements OnInit {

  @Output() closePopup = new EventEmitter();

  public purchasedTickets: TicketPurchased[] = [];
  public userName!: string;
  constructor(private modelService: NgbModal, private sanitizer: DomSanitizer, public ticketPurchaseService: TicketPurchaseService){}

  ngOnInit(): void {
    
  }

  close(){
    this.closePopup.emit();
  }

  getRecipt(id: number){    
    const data = this.purchasedTickets.find(_ => _.id === id)?.recipt?.reciptPdf;
    const pdfDataUrl = 'data:application/pdf;base64,' + data;
    return this.sanitizer.bypassSecurityTrustResourceUrl(pdfDataUrl);    
  }

  viewQr(id: number){
    let purchasedTicket = this.purchasedTickets.find(_ => _.id === id);
    var model = this.modelService.open(ImageViewerComponent, { backdrop: 'static' });
      model.componentInstance.purchasedTicket = purchasedTicket;
      model.componentInstance.closePopup.subscribe(() => {
        model.close();
      })
  }

  deleteTicket(id: number){
    var confirmationModel = this.modelService.open(UserConfirmationPopupComponent, { backdrop: 'static' })
    confirmationModel.componentInstance.content = `Do you want to delete setected ticket for this youser`;
    confirmationModel.componentInstance.command.subscribe((_: boolean) => {
      if(_){
        this.ticketPurchaseService.deleteTicketPurchase(id).subscribe(_ => {
          if(_ === true){
           this.purchasedTickets = this.purchasedTickets.filter(_ => _.id !== id);            
          }
        })
      }
      confirmationModel.close();      
    })
  }
}
