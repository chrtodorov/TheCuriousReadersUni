import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Subject, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';

import { PagingParameters } from '../models/book-parameter.model';
import { CommentRequest } from '../models/comment-request.model';
import { CommentResponse } from '../models/comment-response.model';
import { PagedRequest } from '../models/paged-request.model';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  public pagedCommentsChanged = new Subject<PagedRequest<CommentResponse>>();
  private commentsEndpoint = environment.baseUrl + '/Comments';

  constructor(private http: HttpClient) { }

  public addComment(content: string, bookId: string) {
    const commentRequest = new CommentRequest(content, bookId);
    return this.http.post<CommentResponse>(this.commentsEndpoint, commentRequest).pipe(
      catchError(error => {
        let errorMessage = error.error.message;
        return throwError(() => errorMessage);
      })
    );
  }

  public getCommentsByBookId(bookId: string, params: PagingParameters) {
    let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);

    return this.http.get<PagedRequest<CommentResponse>>(
      `${this.commentsEndpoint}/Book/${bookId}`, {
      params: httpParams
    }).subscribe(pagedComments => {
      this.pagedCommentsChanged.next(pagedComments);
    });
  }

  public getPendingComments(params: PagingParameters) {
    let httpParams = new HttpParams();
    httpParams = httpParams.append('pageNumber', params.pageNumber);
    httpParams = httpParams.append('pageSize', params.pageSize);

    return this.http.get<PagedRequest<CommentResponse>>(
      `${this.commentsEndpoint}/Pending/`, {
      params: httpParams
    }).subscribe(pagedComments => {
      this.pagedCommentsChanged.next(pagedComments);
    });
  }

  public approveComment(commentId: string) {
    return this.http.put<CommentResponse>(
      `${this.commentsEndpoint}/Approve/${commentId}`, {}).pipe(catchError(error => {
        let errorMessage = error.error.message;
        return throwError(() => errorMessage);
      }))
  }

  public rejectComment(commentId: string) {
    return this.http.put<CommentResponse>(
      `${this.commentsEndpoint}/Reject/${commentId}`, {}).pipe(catchError(error => {
        let errorMessage = error.error.message;
        return throwError(() => errorMessage);
      }))
  }
}
