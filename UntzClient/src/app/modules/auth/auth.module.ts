import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProfileComponent } from './profile/profile.component';
import { RegisterComponent } from './register/register.component'

@NgModule({
  declarations: [
    LoginComponent,
    ProfileComponent,
    RegisterComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule      
  ]
})
export class AuthModule { 

}
