import { HttpResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { AuthUser } from 'src/app/models/auth-user';
import { Role } from 'src/app/models/role';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  
  public registerForm!: FormGroup;
  public editMode: boolean = false;
  public user?: AuthUser;
  public action!: string;
  public actionBtn!: string;
  public isActionButtonDisabled: boolean = false;
  public isCreatedByAdmin: boolean = false;
  public roles?: Role[];

  public closePopup = new Subject<AuthUser| undefined>();

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private toastrService: ToastrService, private router: Router){}
    
  ngOnInit(): void {
    this.initializeLoginForm();
    this.action = this.editMode ? 'Update your information' : 'Register';
    this.actionBtn = this.editMode ? 'Update' : 'Register';
  }

  initializeLoginForm(){
    this.registerForm = this.formBuilder.group({
      id: String,
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],      
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{6,}')]],
      role: String,
      isByAdmin: Boolean
    });
    if(this.user){
      this.registerForm.patchValue(this.user);
    }
  }

  onSubmit()
  {    
    this.isActionButtonDisabled = true;
    
    if(this.editMode === true && (this.registerForm.get('firstName')?.value !== undefined
    || this.registerForm.get('lastName')?.value !== undefined
    || this.registerForm.get('phoneNumber')?.value !== undefined
    || this.registerForm.get('email')?.value !== undefined))
    {
      this.authService.updateUser(this.registerForm.value).subscribe(_ => {
        this.user = _;
        this.closePopup.next(_);
      });
    }
    else if(this.registerForm.valid){
        if(this.isCreatedByAdmin)
          this.registerForm.get('isByAdmin')?.setValue(true);
        
        this.authService.register(this.registerForm.value).subscribe((_) => {          
          if(_.userName){    
            
              if(this.isCreatedByAdmin){
                this.toastrService.success("User was created successfully");
                this.closePopup.next(_)
                return;  
              }
              this.toastrService.success(`You were registerd! An email confirmation link was sent to ${this.registerForm.get('email')?.value}. Verify it before login`)           
              this.router.navigate(['auth/login']);
          }
          this.isActionButtonDisabled = false;
      });     
    }
    else{
      this.toastrService.warning("Invalied inputs!. All fields are required");
      this.isActionButtonDisabled = false;
    } 
  }

  close(){
    this.closePopup.next(undefined);
  }
}
