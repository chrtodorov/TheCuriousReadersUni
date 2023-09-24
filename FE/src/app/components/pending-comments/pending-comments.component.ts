import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';

import { PagingParameters } from 'src/app/models/book-parameter.model';
import { CommentResponse } from 'src/app/models/comment-response.model';
import { PagedRequest } from 'src/app/models/paged-request.model';
import { CommentsService } from 'src/app/services/comments.service';

@Component({
  selector: 'app-pending-comments',
  templateUrl: './pending-comments.component.html',
  styleUrls: ['./pending-comments.component.css']
})
export class PendingCommentsComponent implements OnInit, OnDestroy {
  public comments: PagedRequest<CommentResponse>;
  public itemsPerPage = 5;
  public page = 1;
  public totalItems: number;
  public pagingParametes: PagingParameters;
  private pagedCommentsSub: Subscription;

  constructor(
    private commentsService: CommentsService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.pagedCommentsSub = this.commentsService.pagedCommentsChanged.subscribe(pagedComments => {
      this.comments = pagedComments;
      this.totalItems = pagedComments.totalCount;
      if (pagedComments.data.length === 0 && pagedComments.currentPage > 1) {
        this.onPageChange(pagedComments.currentPage - 1);
      }
    });
    this.pagingParametes = new PagingParameters(this.page, this.itemsPerPage);
    this.commentsService.getPendingComments(this.pagingParametes);
  }

  onPageChange(nextPage: number) {
    this.page = nextPage;
    this.pagingParametes = new PagingParameters(this.page, this.itemsPerPage);
    this.commentsService.getPendingComments(this.pagingParametes);
  }

  onApprove(commentId: string) {
    this.commentsService.approveComment(commentId).subscribe({
      next: _ => {
        this.toastr.success('Comment approved successfully');
        this.commentsService.getPendingComments(this.pagingParametes);

      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });
  }

  onReject(commentId: string) {
    this.commentsService.rejectComment(commentId).subscribe({
      next: _ => {
        this.toastr.success('Comment rejected successfully');
        this.commentsService.getPendingComments(this.pagingParametes);
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });

  }

  ngOnDestroy(): void {
    this.pagedCommentsSub.unsubscribe();
  }

}
