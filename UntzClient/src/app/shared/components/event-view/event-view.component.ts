import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { Image } from 'src/app/models/image';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketPurchaseComponent } from '../ticket-purchase/ticket-purchase.component';
import { PresaleSubscribeComponent } from '../presale-subscribe/presale-subscribe.component';

@Component({
  selector: 'app-event-view',
  templateUrl: './event-view.component.html',
  styleUrls: ['./event-view.component.css']
})
export class EventViewComponent implements OnInit {

  public event!: UntzEvent;
  private eventId!: number;
  public image!: string;
  countDownTime?: string = '';
  public isPreSaleAvailable: boolean = false;

  constructor(private eventService: EventService, private activeRoute: ActivatedRoute, private sanitizer: DomSanitizer, private modelService: NgbModal){
  }

  ngOnInit(): void {
    this.activeRoute.params.subscribe(_ => {
      if(_['id']){
        this.eventId = _['id'];
        this.eventService.getEventDetailed(this.eventId).subscribe(_ => {
          this.event = _;

          //Set presale status
          if(_.preSaleStartDate){
            let now = new Date().getTime();
            let preSaleDate = new Date(_.preSaleStartDate!).getTime();

            this.isPreSaleAvailable = (preSaleDate - now) > 0 ? false : true;
          }

          this.setImage(_.images[0]);
          const imageCount = _.images.length;
          var index = 0;

          if(imageCount > 1){
            setInterval(()=>{
              if(index < imageCount){
                this.setImage(_.images[index]);
                ++index;
              }else{
                index = 0;
              }
            },4000);   
          }
          
          this.setCountDown(_.eventStartTime);

          setInterval(() => {
            this.countDownTime
          }, 1000);

        })
      }
    });
  }

  setImage(image: Image){
    if(image){
      const data = image.base64Content;
      const imageDataUrl = 'data:image/jpeg;base64,' + data;
      this.image = imageDataUrl;
    }
  }

  setCountDown(endDate: Date){
    const countDownDate = new Date(endDate).getTime();
    const func = setInterval(() => {
      var now = new Date().getTime();

      var distance = countDownDate - now;

      var days = Math.floor(distance / (1000 * 60 * 60 * 24));
      var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
      var seconds = Math.floor((distance % (1000 * 60)) / 1000);

      this.countDownTime = days + "DAYS " + hours + ":" + minutes + ":" + seconds + " Remaining...";

      if (distance < 0) {
        clearInterval(func);
        this.countDownTime = "Expired";
      }
    }, 1000);
  }

  buy(){
    var model = this.modelService.open(TicketPurchaseComponent, { size: 'lg', backdrop: 'static' });
    model.componentInstance.event = this.event;
    model.componentInstance.closePopup.subscribe(() => {
      model.close();
    })
  }

  subscribe(){
    var model = this.modelService.open(PresaleSubscribeComponent, { size: 'lg', backdrop: 'static' });
    model.componentInstance.event = this.event;
    model.componentInstance.closePopup.subscribe(() => {
      model.close();
    })
  }
}
