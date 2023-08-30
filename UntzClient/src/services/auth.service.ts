import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { AuthUser } from "src/app/models/auth-user";
import { environment } from "src/environments/environment";
import { map, catchError, take } from 'rxjs/operators';
import { Login } from "src/app/models/login";
import { Router } from "@angular/router";
import { Register } from "src/app/models/register";
import { ToastrService } from "ngx-toastr";
import { Role } from "src/app/models/role";

@Injectable()
export class AuthService{

    private backend_service: string = environment.apiUrls.untz_backend_service;
    public authUser?: AuthUser;
    public isAuthenticated = new BehaviorSubject<boolean>(false);

    constructor(private http: HttpClient, private router: Router, private toastrService: ToastrService){
    }

    login(login: Login){
        return this.http.post<any>(`${this.backend_service}/login`, login).subscribe((_) => {
            if(_ === true){
                this.getUser();
                this.router.navigateByUrl('/');
            }
        });
    }

    register(register: Register){
        return this.http.post<AuthUser>(`${this.backend_service}/register`, register);
    }

    logout(){
        return this.http.get<any>(`${this.backend_service}/logout`).subscribe((_) => {
            if(_){
                this.authUser = undefined;
                this.isAuthenticated.next(false);
                this.router.navigate(['auth/login']);
            }
        });
    }

    getUser(): void{
         this.http.get<AuthUser | null>(`${this.backend_service}/untzUsers/current`)
        .pipe(
            map((user) => {
                if(user){
                    this.authUser = user;
                    this.isAuthenticated.next(true);
                }
            }),
            catchError((err) => {
                console.log(err);
                return err;
            })
        ).subscribe();
    }

    getUntzAllUsers(): Observable<any>{
        return this.http.get(`${this.backend_service}/admin/untzUsers`);
    }

    updateUser(user: AuthUser): Observable<AuthUser>{
        return this.http.put<AuthUser>(`${this.backend_service}/untzUsers`, user);
    }

    getAllRoles(): Observable<Role[]>{
        return this.http.get<Role[]>(`${this.backend_service}/admin/roles`);
    }
}