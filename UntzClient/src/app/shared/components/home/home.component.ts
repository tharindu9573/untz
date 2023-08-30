import { Component } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { UntzEvent } from 'src/app/models/event';
import { Image } from 'src/app/models/image';
import { EventService } from 'src/services/event.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  imageDataUrl: any;
  countDownTime?: string = '';
  untzEvent!: UntzEvent;

  constructor(private eventService: EventService, private sanitizer: DomSanitizer, private router: Router){
    eventService.getMainEvent().subscribe(_ => {
      if(_)
      {
        this.setImage(_.images);
        this.setCountDown(_.eventStartTime);
        this.untzEvent = _;
      }    
    });

    setInterval(() => {
      this.countDownTime
    }, 1000);
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

      this.countDownTime = days + "DAYS " + hours + ":" + minutes + ":" + seconds;

      if (distance < 0) {
        clearInterval(func);
        this.countDownTime = "Expired";
      }
    }, 1000);
  }

  navigateToEvent(){
    if(this.untzEvent.id){
      this.router.navigate([`event/${this.untzEvent.id}`]);
    }
  }

  setImage(imageCollection: Image[]){
    if(imageCollection.length > 0){
      const data = imageCollection[0].base64Content;
      const imageDataUrl = 'data:image/jpeg;base64,' + data;
      this.imageDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(imageDataUrl);
    }
  }
}
