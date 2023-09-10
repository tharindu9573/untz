import { Component } from '@angular/core';
import { Router } from '@angular/router';
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

  constructor(private authService: AuthService, private router: Router){
    authService.isAuthenticated.subscribe(_ => {
      this.isAuthenticated = _;
      this.authUser = authService.authUser; 
      this.isAdmin = authService.authUser?.roles.includes('Admin');
    });
  }

  logout(){
    this.authService.logout();
  }

  navigateToEvent(){
    let mainEvent = sessionStorage.getItem("mainEvent");
    if(mainEvent){
      this.router.navigate([`event/${mainEvent}`]);
    }
  }
}
