import { Component } from '@angular/core';
import { UntzEvent } from 'src/app/models/event';
import { EventService } from 'src/services/event.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EventFormComponent } from './event-form/event-form.component';
import { EventConfirmationPopupComponent } from './event-confirmation-popup/event-confirmation-popup.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.css']
})
export class EventComponent {

  public events: UntzEvent[] = [];

  constructor(private eventService: EventService, private modelService: NgbModal, private toastrService: ToastrService){
    this.loadAllEvents();
    eventService.forceReload.subscribe(_ => {
      if(_){
          this.loadAllEvents();
      }
    })
  }

  loadAllEvents(){
    this.eventService.getAllEventsDetailed().subscribe(_ => {
      this.events = _;
    });
  }

  updateEvent(event: UntzEvent){
    var model = this.modelService.open(EventFormComponent, { size: 'lg', backdrop: 'static' });
    model.componentInstance.event = event;
    model.componentInstance.editEvent = true;
    model.componentInstance.closePopup.subscribe(() => {
      model.close();
    });
  }

  addEvent(){
    var model = this.modelService.open(EventFormComponent, { size: 'lg', backdrop: 'static' });
    model.componentInstance.editEvent = false;
    model.componentInstance.closePopup.subscribe(() => {
      model.close();
    });
  }

  deleteEvent(event: UntzEvent){
    var model = this.modelService.open(EventConfirmationPopupComponent, { backdrop: 'static' });
    model.componentInstance.content = "Do you want to delete this event ?"
    model.componentInstance.command.subscribe((_: boolean) =>{
      if(_){
        this.eventService.deleteEvent(event.id).subscribe((_)=>{
          this.toastrService.info("Event deleted");
          this.eventService.forceReload.next(true);
        })
      }
      model.close();
    })
  }

  getEventDescription(description: string){
    var words = description.split(' ');
    words = words.slice(0, 40);
    words.push('...')
    return words.join(' ');
  }
}
