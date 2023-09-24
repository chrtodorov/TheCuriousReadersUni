import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, Subject, throwError } from 'rxjs';

import { environment } from 'src/environments/environment';
import { User } from 'src/app/models/user';
import { BookLoanRequest } from '../models/book-loan-request.model';
import { BookLoan } from '../models/book-loan.model';
import { PagingParameters, BookParameters } from '../models/book-parameter.model';
import { BookCopy, BookViewModel } from '../models/book';
import { BookRequest, BookRequestModified, bookRequestStatusMap } from '../models/book-request.model';
import { BookResponse } from '../models/book-response.model';
import { Book } from '../models/book.model';
import { PagedRequest } from '../models/paged-request.model';
import { ProlongRequest } from '../models/prolong-request.model';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  public booksCount = 0;
  public booksChanged = new Subject<Book[]>();
  public pagedBooksChanged = new Subject<PagedRequest<Book>>();
  public errorOccured = new Subject<string>();
  public bookRequestsChanged = new Subject<PagedRequest<BookRequestModified>>();
  public bookLoansChanged = new Subject<PagedRequest<BookLoan>>();
  public userBookLoansChanged = new Subject<PagedRequest<BookLoan>>();
  private baseUrl = environment.baseUrl + '/Books';
  private genreUrl = environment.baseUrl + '/Genres';
  user: User;
  jwtToken = JSON.parse(sessionStorage.getItem('jwtToken'));

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.jwtToken}`
    })
  }

  constructor(private http: HttpClient) { }

  public getBook(bookId: string) {
    return this.http.get<Book>(`${this.baseUrl}/${bookId}`);
  }

  public getBooks(params: PagingParameters) {
    let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);

    this.http.get<BookResponse>(this.baseUrl, { params: httpParams }).subscribe(response => {
      this.booksCount = response.totalCount;
      this.booksChanged.next(response.data);
    });
  }

  public getFilteredBooks(filterParameters: FilterParameter[],
    pagingParams: PagingParameters,
    filter: string) {

    let bookParameters: BookParameters = new BookParameters();
    let httpParams = new HttpParams();

    if (filterParameters.includes('title')) {
      bookParameters.title = filter;
      httpParams = httpParams.append('title', bookParameters.title);
    }

    if (filterParameters.includes('author')) {
      bookParameters.author = filter;
      httpParams = httpParams.append('author', bookParameters.author);
    }

    if (filterParameters.includes('descriptionKeyword')) {
      bookParameters.descriptionKeyword = filter;
      httpParams = httpParams.append('descriptionKeyword', bookParameters.descriptionKeyword);
    }

    if (filterParameters.includes('publisher')) {
      bookParameters.publisher = filter;
      httpParams = httpParams.append('publisher', bookParameters.publisher);
    }

    if (filterParameters.includes('genre')) {
      bookParameters.genre = filter;
      httpParams = httpParams.append('genre', bookParameters.genre);
    }
    httpParams = httpParams.append('pageNumber', pagingParams.pageNumber);
    httpParams = httpParams.append('pageSize', pagingParams.pageSize);

    this.http.get<BookResponse>(this.baseUrl, { params: httpParams })
      .pipe(catchError(this.errorHandler))
      .subscribe({
        next: (response) => {
          this.booksCount = response.totalCount;
          this.booksChanged.next(response.data);
        }, error: (error) => {
          this.errorOccured.next(error.toString());
        }
      });
  }

  public getBookRequests(params: PagingParameters) {
    return this.pagedRequest<BookRequest>(`${environment.baseUrl}/BookRequests`, params)
      .pipe(map(this.getModifiedBookRequests))
      .subscribe(bookRequests => {
        this.bookRequestsChanged.next(bookRequests);
      }
      );
  }

  public getCurrentUsersBookRequests(params: PagingParameters) {
    return this.pagedRequest<BookRequest>(`${environment.baseUrl}/BookRequests/Mine`, params)
      .pipe(map(this.getModifiedBookRequests))
      .subscribe(bookRequests => {
        this.bookRequestsChanged.next(bookRequests);
      }
      );
  }

  public reserveBook(bookId: string) {
    return this.http.post<BookRequest>(`${environment.baseUrl}/BookRequests`, { bookId })
      .pipe(catchError(this.errorHandler));
  }

  public rejectRequest(bookRequestId: string) {
    return this.http.put(`${environment.baseUrl}/BookRequests/${bookRequestId}`, {})
      .pipe(catchError(this.errorHandler));
  }

  public makeLoan(bookLoan: BookLoanRequest) {
    return this.http.post<BookLoan>(`${environment.baseUrl}/BookLoans`, bookLoan)
      .pipe(catchError(this.errorHandler)
      );
  }

  public getAllLoans(params: PagingParameters) {
    return this.pagedRequest<BookLoan>(`${environment.baseUrl}/BookLoans`, params)
      .subscribe(loans => {
        this.bookLoansChanged.next(loans);
      });
  }

  public getExpiringLoans(params: PagingParameters) {
    return this.pagedRequest<BookLoan>(`${environment.baseUrl}/BookLoans/expiring`, params)
      .subscribe(loans => {
        this.bookLoansChanged.next(loans);
      });
  }
  getLatest() {
    return this.http.get<Book[]>(this.baseUrl + '/latestbooks')
  }
  makeUnavailable(id: string) {
    return this.http.put(this.baseUrl + '/' + id + '/status', this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
  }

  getGenreCount() {
    return this.http.get(this.genreUrl + '/count')
  }

  getCount() {
    return this.http.get(this.baseUrl + '/count')
  }

  getGenres(): Observable<any> {
    return this.http.get(this.genreUrl)

      .pipe(
        catchError(this.errorHandler)
      );
  }

  public getCurrentUsersLoans(params: PagingParameters) {
    return this.pagedRequest<BookLoan>(`${environment.baseUrl}/BookLoans/Mine`, params)
      .subscribe(pagedRequest => {
        this.bookLoansChanged.next(pagedRequest);
      });
  }

  public getLoansByUserId(userId: string, params: PagingParameters) {
    return this.pagedRequest<BookLoan>(`${environment.baseUrl}/BookLoans/User/${userId}`, params)
      .pipe(catchError(this.errorHandler))
      .subscribe(pagedRequest => this.userBookLoansChanged.next(pagedRequest));
  }

  public prolongLoan(loanId: string, prolongRequest: ProlongRequest) {
    return this.http.put<BookLoan>(`${environment.baseUrl}/BookLoans/${loanId}`, prolongRequest)
      .pipe(catchError(this.errorHandler));
  }

  public completeLoan(loanId: string, bookId: string, loanedToId: string) {
    return this.http.put(`${environment.baseUrl}/BookLoans/Complete/${loanId}`, { bookId, loanedToId })
      .pipe(catchError(this.errorHandler));
  }

  public getCurrentUserReadBooks(params: PagingParameters) {
    return this.pagedRequest<Book>(`${this.baseUrl}/Read`, params).subscribe(pagedBooks => {
      this.pagedBooksChanged.next(pagedBooks);
    });
  }

  private getModifiedBookRequests(bookRequests: PagedRequest<BookRequest>) {
    const modifiedData = bookRequests.data.map(bookRequest => {
      const dateModified = new Date(bookRequest.createdAt?.toString())
        .toLocaleDateString(undefined, { year: "numeric", month: "long", day: "numeric" });

      return new BookRequestModified(
        bookRequest.bookRequestId,
        dateModified,
        bookRequestStatusMap.get(bookRequest.status),
        bookRequest.book,
        bookRequest.requestedById,
        bookRequest.bookCopyId,
        bookRequest.requestedBy);
    });

    return new PagedRequest(
      bookRequests.currentPage,
      bookRequests.totalPages,
      bookRequests.pageSize,
      bookRequests.totalCount,
      bookRequests.hasPrevious,
      bookRequests.hasNext,
      modifiedData
    )
  }

  private pagedRequest<T>(url: string, params: PagingParameters) {
    let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);
    return this.http.get<PagedRequest<T>>(url, { params: httpParams });
  }



  getById(id: string): Observable<BookViewModel> {
    return this.http.get<BookViewModel>(this.baseUrl + '/' + id)
      .pipe(
        catchError(this.errorHandler)
      )
  }

  getBookItem(id: string): Observable<BookCopy> {
    return this.http.get<BookCopy>(this.baseUrl + '/' + id)
  }

  create(book: Book): Observable<any> {
    return this.http.post(this.baseUrl, JSON.stringify(book), this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
  }

  update(id: string, book: Book): Observable<any> {
    return this.http.put(this.baseUrl + '/' + id, JSON.stringify(book), this.httpOptions)
      .pipe(
        catchError(this.errorHandler)
      )
  }

  delete(id: string) {
    return this.http.delete(this.baseUrl + '/' + id, this.httpOptions)

      .pipe(
        catchError(this.errorHandler)
      )
  }

  errorHandler(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    }

    else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.error.message}`;
    }

    return throwError(() => errorMessage);
  }
}

export type FilterParameter = "title" | "author" | "descriptionKeyword" | "publisher" | "genre";