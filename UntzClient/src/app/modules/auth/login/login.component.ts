import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public loginForm!: FormGroup;
  public isLoginButtonIsDisabled: boolean = false;

  constructor(private formBuilder: FormBuilder, private authService: AuthService, private router: Router, private toastrService: ToastrService){
    this.initializeLoginForm();
  }

  initializeLoginForm(){
    this.loginForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{6,}')]]
    });
  }

  onSubmit(){
    if(this.loginForm.valid){
      this.isLoginButtonIsDisabled = true;
      this.authService.login(this.loginForm.value).add(() => {
        console.log("Teardown")
        this.isLoginButtonIsDisabled = false;
      });
    }else{
      this.toastrService.warning("Invalid!");
    }
  }

  navigateToRegister(){
    this.router.navigateByUrl("auth/register");
  }
}
