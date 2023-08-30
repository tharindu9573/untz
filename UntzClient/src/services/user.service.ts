import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthUser } from 'src/app/models/auth-user';
import { GuestUser } from 'src/app/models/guest-user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private backend_service: string = environment.apiUrls.untz_backend_service;
  
  constructor(private http: HttpClient) { }

  getAllUntzUsers(): Observable<AuthUser[]>{
    return this.http.get<AuthUser[]>(`${this.backend_service}/admin/untzUsers`);
  }

  getGuestUsers(): Observable<GuestUser[]>{
    return this.http.get<GuestUser[]>(`${this.backend_service}/admin/guestUsers`);
  }

  createGuestUser(guestUser: GuestUser){
    return this.http.post<GuestUser>(`${this.backend_service}/guestUsers`, guestUser);
  }

  deleteUntzUser(id: string){
    return this.http.delete<boolean>(`${this.backend_service}/admin/untzUsers/${id}/delete`);
  }

  deleteGuestUser(id: number){
    return this.http.delete<boolean>(`${this.backend_service}/admin/guestUsers/${id}/delete`);
  }

}
