import { Component, OnInit } from '@angular/core';
import { ForgotPassword } from 'src/app/models/forgot-password';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  public forgotPasswordForm: FormGroup
  public successMessage: string;
  public errorMessage: string;
  public showSuccess: boolean;
  public showError: boolean;


  constructor(private authService: AuthenticationService) { }

  ngOnInit(): void {
    this.forgotPasswordForm = new FormGroup({
      email: new FormControl("", [Validators.required])
    })
  }

  public validateControl = (controlName: string) => {
    return this.forgotPasswordForm.controls[controlName].invalid && this.forgotPasswordForm.controls[controlName].touched
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.forgotPasswordForm.controls[controlName].hasError(errorName)
  }
  public forgotPassword = (forgotPasswordFormValue: any) => {
    this.showError = this.showSuccess = false;
    const forgotPass = { ...forgotPasswordFormValue };
    const forgotPassword: ForgotPassword = {
      email: forgotPass.email,
      clientURI: 'http://localhost:4200/...' //TO DO
    }
    
    this.authService.forgotPassword(forgotPassword).subscribe(_ => {
      this.showSuccess = true;
      this.successMessage = "The link has been sent, please check your email to reset your password."
    },
    err => {
      this.showError = true,
      this.errorMessage = err;
    })
  }

  
}
