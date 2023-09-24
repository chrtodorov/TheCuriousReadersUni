import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Login } from 'src/app/models/login';
import { UserViewModel } from 'src/app/models/user';

@Injectable({
  providedIn: 'root'
})

export class ProfileService {
    profileURL = environment.PROFILE_API;
    profileLIbrarianURL = environment.PROFILE_LIBRARIAN_API;
    jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));
    
    user: Login;
    jwt = sessionStorage.getItem('jwtToken');
    jwtData = this.jwt.split('.')[1]
    decodedJwtJsonData = window.atob(this.jwtData)
    decodedJwtData = JSON.parse(this.decodedJwtJsonData)

    userId = this.decodedJwtData["Id"];

    httpOptions = {
        headers: new HttpHeaders({
          'Authorization': `Bearer ${this.jwtToken}`
        })
      }
      
      constructor(private http:HttpClient){}

    getUserById(userId: string): Observable<UserViewModel>{
        return this.http.get<UserViewModel>(this.profileURL + '/' + userId, this.httpOptions)
    }
    getLibrarianById(librarianId: string): Observable<UserViewModel>{
      return this.http.get<UserViewModel>(this.profileLIbrarianURL + '/' + librarianId, this.httpOptions)
    }
}