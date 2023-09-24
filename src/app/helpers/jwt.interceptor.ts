import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';
import { Login } from 'src/app/models/login';


@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private authService: AuthenticationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let currentUser: Login;

    this.authService.currentUser$
    .pipe(take(1))
    .subscribe((user) => currentUser = user);
    
    if(currentUser){
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.jwtToken}`
        }
      })
    }
    
    return next.handle(request);
  }
}
