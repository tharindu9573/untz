import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, finalize, of, tap, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { ToastrService } from 'ngx-toastr';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptorService implements HttpInterceptor {

  constructor(private router: Router, private authService: AuthService, private toastrService: ToastrService, private loadingService: LoadingService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if(req.method === 'GET'){
      this.loadingService.setLoading(true);
    }
    
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if(error.status == 401){
          this.authService.isAuthenticated.next(false);
          this.toastrService.error("Unauthorized!");
          this.router.navigate(["/auth/login"]);
        }
        else{
          let errorMessage = error.error[0]?.description;
          this.toastrService.error(errorMessage ?? "Error occured!"); 
        }
        
        let response =  new HttpResponse({
          status: error.status,
          body: { error: error.error[0]?.description}
        });
        return of(response);        
      }),
      finalize(() => {         
        this.loadingService.setLoading(false);        
      }),
    )
  }
}
