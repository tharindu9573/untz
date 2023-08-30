import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/services/auth.service';
import { HttpClientModule, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http'
import { AuthModule } from './modules/auth/auth.module';
import { AuthInterceptorService } from 'src/services/auth-interceptor.service';
import { ToastrModule } from 'ngx-toastr'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { EventService } from 'src/services/event.service';
import { NavBarComponent } from './shared/components/nav-bar/nav-bar.component';
import { HomeComponent } from './shared/components/home/home.component';
import { AdminModule } from './modules/admin/admin.module';
import { EventComponent } from './shared/components/event/event.component';
import { EventViewComponent } from './shared/components/event-view/event-view.component';
import { TicketPurchaseComponent } from './shared/components/ticket-purchase/ticket-purchase.component';
import { TicketService } from 'src/services/ticket.service';
import { FormsModule } from '@angular/forms';
import { TicketPurchaseService } from 'src/services/ticket-purchase.service';
import { PurchasedTicketComponent } from './shared/components/purchased-ticket/purchased-ticket.component';
import { AboutComponent } from './shared/components/about/about.component';
import { LoaderComponent } from './shared/components/loader/loader.component';
import { LoadingService } from 'src/services/loading.service';
import { PurchasedTicketDetailedComponent } from './shared/components/purchased-ticket-detailed/purchased-ticket-detailed.component';
import { TicketViewComponent } from './shared/components/ticket-view/ticket-view.component';
import { PresaleSubscribeComponent } from './shared/components/presale-subscribe/presale-subscribe.component';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    EventComponent,
    EventViewComponent,
    TicketPurchaseComponent,
    PurchasedTicketComponent,
    AboutComponent,
    LoaderComponent,
    PurchasedTicketDetailedComponent,
    TicketViewComponent,
    PresaleSubscribeComponent  
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    AuthModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    AdminModule,
    FormsModule
  ],
  providers: [
    AuthService, 
    HttpClient,
    EventService,
    TicketService,
    TicketPurchaseService,
    LoadingService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true 
    }
  ],

  bootstrap: [AppComponent]
})
export class AppModule { }
