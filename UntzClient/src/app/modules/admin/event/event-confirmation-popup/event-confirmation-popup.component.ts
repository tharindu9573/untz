import { Component } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-event-confirmation-popup',
  templateUrl: './event-confirmation-popup.component.html',
  styleUrls: ['./event-confirmation-popup.component.css']
})
export class EventConfirmationPopupComponent {
  public content!: string;

  public command = new Subject<boolean>();

  constructor(){}

  yes() {
   this.command.next(true)
  }

  no() {
    this.command.next(false);
  }
}
