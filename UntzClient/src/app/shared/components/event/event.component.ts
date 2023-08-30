import { Component } from '@angular/core';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { Image } from 'src/app/models/image';
import { DomSanitizer } from '@angular/platform-browser';
import { Router } from '@angular/router';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.css']
})
export class EventComponent {
  
  public events: UntzEvent[] = [];
  constructor(private eventService: EventService, private sanitizer: DomSanitizer, private router: Router){
    eventService.getAllEventsDetailed().subscribe(_ => {
      this.events = _;
    });
  }

  getImage(imageCollection: Image[]): any{
    if(imageCollection.length > 0){
      const data = imageCollection[0].base64Content;
      const imageDataUrl = 'data:image/jpeg;base64,' + data;
      return this.sanitizer.bypassSecurityTrustResourceUrl(imageDataUrl);
    }
    return '';
  }

  navigateToEvent(id: number){
    this.router.navigate([`event/${id}`]);
  }
}
