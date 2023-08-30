import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaymentMethod } from 'src/app/models/payment-method';
import { Ticket } from 'src/app/models/ticket';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private backend_service: string = environment.apiUrls.untz_backend_service;

  constructor(private httpClient: HttpClient) { }

  getTicketsForEvent(eventId: number): Observable<Ticket[]>{
    return  this.httpClient.get<Ticket[]>(`${this.backend_service}/tickets/${eventId}`);
  }

  getPaymentMethods(): Observable<PaymentMethod[]>{
    return  this.httpClient.get<PaymentMethod[]>(`${this.backend_service}/paymentMethods`);
  }
}
