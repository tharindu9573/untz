import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { Image } from 'src/app/models/image';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketPurchaseComponent } from '../ticket-purchase/ticket-purchase.component';
import { PresaleSubscribeComponent } from '../presale-subscribe/presale-subscribe.component';
import { Countdown } from 'src/utility/countdown';

@Component({
  selector: 'app-event-view',
  templateUrl: './event-view.component.html',
  styleUrls: ['./event-view.component.css']
})
export class EventViewComponent implements OnInit {

  public event!: UntzEvent;
  private eventId!: number;
  public image?: string;
  countDownTime?: string = '';
  public isPreSaleAvailable: boolean = false;

  constructor(private eventService: EventService, private activeRoute: ActivatedRoute, private modelService: NgbModal){
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
          
          Countdown.setCountDown(_.eventStartTime).subscribe(_ => {
            this.countDownTime = _;
          });

          setInterval(() => {
            this.countDownTime
          }, 1000);

        })
      }
    });
  }

  setImage(image: Image){
    if(image){
      this.image = image?.filePath;
    }
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
