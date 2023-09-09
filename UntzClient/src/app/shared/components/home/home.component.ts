import { AfterViewInit, Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { Countdown } from 'src/utility/countdown';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{
  countDownTime?: string = '';
  untzEvent!: UntzEvent;
  imageVisible: boolean = true;
  date?: string;
  month?: string;
  year?: string;

  constructor(private eventService: EventService, private router: Router){
    
  }

  onVideoLoaded(){
    var video = document.getElementById("teaser") as HTMLVideoElement;
    var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    if (isMobile) {
      video.removeAttribute("controls");
    }
    video.onloadeddata = () => {
      setTimeout(() => {
        video.play();
        this.imageVisible = false; 
      }, 5000);             
    };
  }

  ngOnInit(): void {

    this.eventService.getMainEvent().subscribe(_ => {
      if(_)
      {
        this.untzEvent = _;      
        Countdown.setCountDown(_.eventStartTime).subscribe(_ => {
          this.countDownTime = _;
        });        
      }    
    });

    setInterval(() => {
      this.countDownTime
    }, 1000);
  }

  navigateToEvent(){
    if(this.untzEvent.id){
      this.router.navigate([`event/${this.untzEvent.id}`]);
    }
  }
}
