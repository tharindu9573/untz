import { Component } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-user-confirmation-popup',
  templateUrl: './user-confirmation-popup.component.html',
  styleUrls: ['./user-confirmation-popup.component.css']
})
export class UserConfirmationPopupComponent {

  public command = new Subject<boolean>();
  public content!: string;
  
  constructor(){}

  yes() {
    this.command.next(true)
   }
 
  no() {
    this.command.next(false);
  }
}
