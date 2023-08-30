import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaymentGateway } from 'src/app/models/payment-gateway';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TicketPurchaseService {

  private backend_service: string = environment.apiUrls.untz_backend_service;
  
  constructor(private httpClient: HttpClient) { }

  getPurchasedTicketsForCurrentUser(): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/purchase`);
  }

  getPurchasedTicketsForUntzUser(id: string): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/admin/purchase/user/${id}`);
  }

  getPurchasedTicketsForGuestUser(id: number): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/admin/purchase/guestuser/${id}`);
  }

  getPurchasedTicketByReference(referenceId : string): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/purchase/${referenceId}`);
  }

  getPurchasedTicketByReferenceDetailed(referenceId : string): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/purchase/${referenceId}/detailed`);
  }

  GenerateBodyWithHash(ticket: any): Observable<PaymentGateway>{ 
    return  this.httpClient.post<PaymentGateway>(`${this.backend_service}/purchase`, ticket);
  }

  getPurchasedTicketAloneByReference(purchasedReferenceId: string, ticketReferenceId: string): Observable<string>{
    return  this.httpClient.get<string>(`${this.backend_service}/purchase/${purchasedReferenceId}/ticket/${ticketReferenceId}`);
  }

  deleteTicketPurchase(ticketPurchaseId: number){
    return this.httpClient.delete<boolean>(`${this.backend_service}/admin/purchase/${ticketPurchaseId}/delete`)
  }

  admit(referenceId: number){
    return this.httpClient.get<boolean>(`${this.backend_service}/purchase/${referenceId}/admit`);
  }
}
