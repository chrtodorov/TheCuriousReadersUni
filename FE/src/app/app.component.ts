import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Login } from 'src/app/models/login';
import { AuthenticationService } from './services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'TheCuriousReaders';
  role:string;
  user:Login;
  constructor(public authService: AuthenticationService, public router:Router){}
  ngOnInit(): void {
    this.setRole();
    this.isLoggedIn();
    
    this.authService.userStatusChanged.subscribe(() => {
      this.setRole();
      this.isLoggedIn();
    });
  }
  
  setRole(){
    const role: Login = JSON.parse(sessionStorage.getItem('role'));
    this.role = this.authService.getRole();
    return role;
  }
  isLoggedIn(){
    const user: Login = JSON.parse(sessionStorage.getItem('user'));
    this.authService.setCurrentUser(user);
    this.user = user;
    return user;
  }
  logout(){
      this.authService.logout();
      this.router.navigateByUrl('/');
  }
}