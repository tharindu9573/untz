import { Component } from '@angular/core';
import { AuthUser } from 'src/app/models/auth-user';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent {

  public isAuthenticated: boolean = false;
  public authUser?: AuthUser;
  public isAdmin?: boolean = false;

  constructor(private authService: AuthService){
    authService.isAuthenticated.subscribe(_ => {
      this.isAuthenticated = _;
      this.authUser = authService.authUser; 
      this.isAdmin = authService.authUser?.roles.includes('Admin');
    });
  }

  logout(){
    this.authService.logout();
  }
}
