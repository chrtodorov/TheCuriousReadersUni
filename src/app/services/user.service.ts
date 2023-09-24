import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Login } from 'src/app/models/login';
import { User } from '../models/user.model';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.baseUrl + '/Users'
  constructor(private http:HttpClient) { }

  currentUser: Login

  getUser()
  {
    let jwt = this.currentUser.jwtToken
    let jwtData = jwt.split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)

    let role = decodedJwtData.role
  }

  getUserId()
  {
    let jwt = this.currentUser.jwtToken
    let jwtData = jwt.split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)

    let id = decodedJwtData.Id
    return id;
  }

  getUsers(filter: string) {
    return this.http.get<User[]>(`${environment.baseUrl}/Users`, {
      params: new HttpParams().append('filter', filter)
    })
  }
  getUserCount()
  {
    return this.http.get(this.baseUrl + '/count')
  }
}
