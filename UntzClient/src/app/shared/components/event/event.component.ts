import { Component } from '@angular/core';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.css']
})
export class EventComponent {
  
  public events: UntzEvent[] = [];
  constructor(private eventService: EventService, private router: Router){
    eventService.getAllEventsDetailed().subscribe(_ => {
      this.events = _;
    });
  }

  navigateToEvent(id: number){
    this.router.navigate([`event/${id}`]);
  }

  getEventDescription(description: string){
    var words = description.split(' ');
    words = words.slice(0, 40);
    words.push('...')
    return words.join(' ');
  }
}
