import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import {  Observable, Subject, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Author, AuthorPagingParameters, AuthorParameters, AuthorResponse } from './author';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {
    private apiURL = environment.baseUrl + '/Authors';

    public authorsCount = 0;
    public authorsChanged = new Subject<Author[]>();
    public errorOccured = new Subject<string>();

    jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));

    httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.jwtToken}`
      })
    }

    constructor(private httpClient: HttpClient) { }

    getAll(params: AuthorPagingParameters) {
      let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);

    this.httpClient.get<AuthorResponse>(this.apiURL, {params: httpParams}).subscribe(response => {
      this.authorsCount = response.totalCount;
      this.authorsChanged.next(response.data);
    });
    }
    getFiltered(filterParameters: FilterParameter[], pagingParams: AuthorPagingParameters, filter: string){
      let authorParameters: AuthorParameters = new AuthorParameters();
      let httpParams = new HttpParams();

      if(filterParameters.includes('name')){
        authorParameters.name = filter;
        httpParams = httpParams.append('name', authorParameters.name);
      }
      httpParams = httpParams.append('pageNumber', pagingParams.pageNumber);
      httpParams = httpParams.append('pageSize', pagingParams.pageSize);
      console.log(httpParams);

      this.httpClient.get<AuthorResponse>(this.apiURL, {params: httpParams})
        .pipe(catchError(this.errorHandler))
        .subscribe({
          next: (response) => {
            this.authorsCount = response.totalCount;
            this.authorsChanged.next(response.data);
          }, error: (error) => {
            this.errorOccured.next(error.toString());
          }
        })
    }

    getById(id:string): Observable<any> {
      return this.httpClient.get(this.apiURL + '/' + id)

      .pipe(
        catchError(this.errorHandler)
      )
    }

    create(author:Author): Observable<any>{
      return this.httpClient.post(this.apiURL, JSON.stringify(author), this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
    }

    update(id:string, author:Author): Observable<any> {
      return this.httpClient.put(this.apiURL + '/' + id, JSON.stringify(author), this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
    }

    delete(id:string){
      return this.httpClient.delete(this.apiURL + '/' + id, this.httpOptions)
      
      .pipe(
        catchError(this.errorHandler)
      )
    }

    errorHandler(error:any) {
    let errorMessage = '';
    if(error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.error.message}`;
    }
    return throwError(() => errorMessage);
  }
}

export type FilterParameter = 'name' | 'bio';