import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { UntzEvent } from 'src/app/models/event';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  private backend_service: string = environment.apiUrls.untz_backend_service;
  public forceReload = new BehaviorSubject(false);
  
  constructor(private http: HttpClient) { }

  getMainEvent(): Observable<UntzEvent>{
    return this.http.get<UntzEvent>(`${this.backend_service}/events/mainEvent`);
  }

  getAllEventsDetailed(): Observable<UntzEvent[]>{
    return this.http.get<UntzEvent[]>(`${this.backend_service}/events/detailed`);
  }

  getEventDetailed(id: number): Observable<UntzEvent>{
    return this.http.get<UntzEvent>(`${this.backend_service}/events/${id}/detailed`);
  }

  getAllEvents(): Observable<UntzEvent[]>{
    return this.http.get<UntzEvent[]>(`${this.backend_service}/events`);
  }

  updateEvent(event: UntzEvent){
    return this.http.put<UntzEvent>(`${this.backend_service}/admin/events`, event);
  }

  addEvent(event: UntzEvent){
    return this.http.post<UntzEvent>(`${this.backend_service}/admin/events`, event);
  }

  deleteEvent(id: number){
    return this.http.delete<boolean>(`${this.backend_service}/admin/events/${id}`);
  }

  subscribe(body: any){
    return this.http.post<boolean>(`${this.backend_service}/events/subscribe`, body);
  }
}
