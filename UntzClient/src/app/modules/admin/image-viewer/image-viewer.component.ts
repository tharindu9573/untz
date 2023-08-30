import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { QrCode } from 'src/app/models/qr-code';
import { TicketPurchased } from 'src/app/models/ticket-purchased';

@Component({
  selector: 'app-image-viewer',
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.css']
})
export class ImageViewerComponent implements OnInit{
  
  public purchasedTicket!: TicketPurchased;
  public imageData?: QrCode[];

  @Output() closePopup = new EventEmitter();

  constructor(private sanitizer: DomSanitizer){}

  ngOnInit(): void {
    if(this.purchasedTicket){
      this.imageData = this.purchasedTicket.qrCode;
    }
  }

  getImage(base64Content: string): any{
    const data = base64Content;
    const imageDataUrl = 'data:image/jpeg;base64,' + data;
    return this.sanitizer.bypassSecurityTrustResourceUrl(imageDataUrl);
  }

  close(){
    this.closePopup.emit();
  }
}
