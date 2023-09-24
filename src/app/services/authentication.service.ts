import { Injectable } from '@angular/core';
import { map, ReplaySubject, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Login } from 'src/app/models/login';
import { ForgotPassword } from 'src/app/models/forgot-password';
import { Router } from '@angular/router';
import { User } from 'src/app/models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  baseUrl = environment.AUTH_API;
  registerUrl = environment.REGISTER_API;
  
  public userStatusChanged = new Subject<void>();
  private currentUserSource = new ReplaySubject<Login>(1);
  private currentUserRegisterSource = new ReplaySubject<User>(1);

  currentUserRegister$ = this.currentUserRegisterSource.asObservable();
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private router:Router){}
  
    login(model: any)
    {
      return this.http.post<Login>(this.baseUrl, model).pipe(
        map((response: Login) => {
          const user = response;
          if(user){
            sessionStorage.setItem('user', JSON.stringify(user));
            sessionStorage.setItem('jwtToken', JSON.stringify(user.jwtToken));
            sessionStorage.setItem('role', JSON.stringify(user.role));
            this.currentUserSource.next(user);
            this.userStatusChanged.next();
          }
        }) 
      )
    }

    register(model: any){
      return this.http.post<User>(this.registerUrl, model).pipe(
        map((user:User) =>{
          if(user){
            this.setCurrentUserRegister(user);
          }
        })
      )
    }

    setCurrentUser(user: Login)
    {
      sessionStorage.setItem('user', JSON.stringify(user));
      this.currentUserSource.next(user);
    }

    setCurrentUserRegister(user: User){
      sessionStorage.setItem('registeredUser', JSON.stringify(user));
      this.currentUserRegisterSource.next(user);
    }
  
    logout(){
      sessionStorage.removeItem('user');
      sessionStorage.removeItem('jwtToken');
      sessionStorage.removeItem('role');
      this.currentUserSource.next(null);
      this.userStatusChanged.next();

    }

    forgotPassword = (body: ForgotPassword) => {
      return this.http.post((this.baseUrl + '/ForgotPassword'),body)
    }
    
    getRole(){
      return JSON.parse(sessionStorage.getItem('role'));
    }
    
    
}
