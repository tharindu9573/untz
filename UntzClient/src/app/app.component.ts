import { Component } from '@angular/core';
import { AuthService } from 'src/services/auth.service';
import { EventService } from 'src/services/event.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent{
  title = 'UntzClient';

  isAuthenticated: Boolean = false;
  constructor(private authService: AuthService, private eventService: EventService){
    authService.getUser();
    authService.isAuthenticated.subscribe(_ => {
      this.isAuthenticated = _; 
    });    
  }
}
