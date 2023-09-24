import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import {  Observable, Subject, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Publisher, PublisherPagingParameters, PublisherParameters, PublisherResponse } from './publisher';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PublisherService {
    private apiURL = environment.baseUrl + '/Publishers';

    public publishersCount = 0;
    public publishersChanged = new Subject<Publisher[]>();
    public errorOccured = new Subject<string>();

    jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));

    httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.jwtToken}`
      })
    }

    constructor(private httpClient: HttpClient) { }

    getAll(params: PublisherPagingParameters) {
      let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);

    this.httpClient.get<PublisherResponse>(this.apiURL, {params: httpParams}).subscribe(response => {
      this.publishersCount = response.totalCount;
      this.publishersChanged.next(response.data);
    });
    }
    getFiltered(filterParameters: FilterParameter[], pagingParams: PublisherPagingParameters, filter: string){
      let publisherParameters: PublisherParameters = new PublisherParameters();
      let httpParams = new HttpParams();

      if(filterParameters.includes('name')){
        publisherParameters.name = filter;
        httpParams = httpParams.append('name', publisherParameters.name);
      }
      httpParams = httpParams.append('pageNumber', pagingParams.pageNumber);
      httpParams = httpParams.append('pageSize', pagingParams.pageSize);
      console.log(httpParams);

      this.httpClient.get<PublisherResponse>(this.apiURL, {params: httpParams})
        .pipe(catchError(this.errorHandler))
        .subscribe({
          next: (response) => {
            this.publishersCount = response.totalCount;
            this.publishersChanged.next(response.data);
          }, error: (error) => {
            this.errorOccured.next(error.toString());
          }
        })
    }

    getById(id:string): Observable<Publisher> {
      return this.httpClient.get<Publisher>(this.apiURL + '/' + id)

      .pipe(
        catchError(this.errorHandler)
      )
    }

    create(publisher:Publisher): Observable<any>{
      return this.httpClient.post(this.apiURL, JSON.stringify(publisher), this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
    }

    update(id:string, publisher:Publisher): Observable<any> {
      return this.httpClient.put(this.apiURL + '/' + id, JSON.stringify(publisher), this.httpOptions)

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
    }

    else {
      console.log(error);
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.error.message}`;      
    }
    
    return throwError(() => errorMessage);
  }
}

export type FilterParameter = 'name';