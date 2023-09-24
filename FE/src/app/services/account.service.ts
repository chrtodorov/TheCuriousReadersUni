import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User, UserViewModel } from 'src/app/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountSerivce {
  approveURL = environment.APPROVE_API;
  rejectURL = environment.REJECT_API;
  getPendingCustomersURL = environment.GET_PENDING_CUSTOMERS_API;
  getPendingURL = environment.GET_PENDING_API;

  user: User;
  jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));

  httpOptions = {
    headers: new HttpHeaders({
      'Authorization': `Bearer ${this.jwtToken}`
    })
  }

  constructor(private http: HttpClient) {}

  getAllPending(): Observable<UserViewModel[]> {  //Admin
    return this.http.get<UserViewModel[]>(this.getPendingURL, this.httpOptions);
  }

  getPendingCustomers(): Observable<UserViewModel[]> {  //Librarian
    return this.http.get<UserViewModel[]>(this.getPendingCustomersURL, this.httpOptions);
  }

  approve(userId: string): Observable<any>{
    return this.http.put(this.approveURL + '/' + userId, JSON.stringify(this.user), this.httpOptions)
    .pipe(catchError(this.errorHandler))
  }
  
  reject(userId: string): Observable<any>{
    return this.http.put(this.rejectURL + '/' + userId, JSON.stringify(this.user), this.httpOptions)
    .pipe(catchError(this.errorHandler))
  }

  errorHandler(error: any){
    let errorMessage = '';
    if(error.error instanceof ErrorEvent){
      errorMessage = error.error.message;
    }
    else{
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.error.message}`;
    }
    return throwError(() => errorMessage);
  }
}
