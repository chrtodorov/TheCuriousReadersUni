import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/services/authentication.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  model: any = {};

  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  roles: string[] = [];

  constructor(public authService: AuthenticationService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void { }

  login() {
    this.authService
      .login(this.model)
      .subscribe(response => {
        this.router.navigateByUrl('/');
        this.toastr.success('Logged in successfully!');

      }, error => {
        console.log(error);
        this.toastr.error('Incorrect credentials! Please try again!');
      })
  }

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('/');
  }
}
