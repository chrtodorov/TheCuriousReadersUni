import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { BookCopy, BookViewModel } from '../../models/book';
import { BookService } from '../../services/book.service';
import { NgForm } from '@angular/forms';
import { CommentsService } from 'src/app/services/comments.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { PagedRequest } from 'src/app/models/paged-request.model';
import { CommentResponse } from 'src/app/models/comment-response.model';
import { Subscription } from 'rxjs';
import { PagingParameters } from 'src/app/models/book-parameter.model';


@Component({
  selector: 'app-bookdetails',
  templateUrl: './bookdetails.component.html',
  styleUrls: ['./bookdetails.component.css'],
})
export class BookdetailsComponent implements OnInit, OnDestroy {
  copies: BookCopy[] = [];
  role: string;
  bookId: any;
  book: BookViewModel = new BookViewModel();
  comments: PagedRequest<CommentResponse>;
  itemsPerPage = 5;
  page = 1;
  totalItems: number;
  pagingParametes: PagingParameters;
  private currentLoadedBookId: string;
  private modalRef: NgbModalRef;
  private pagedCommentsSub: Subscription;

  constructor(public bookService: BookService,
    public router: Router,
    private route: ActivatedRoute,
    public authService: AuthenticationService,
    private toastr: ToastrService,
    private commentsService: CommentsService,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.getById();
    this.checkRole(this.role);
    this.pagedCommentsSub = this.commentsService.pagedCommentsChanged.subscribe(pagedComments => {
      this.comments = pagedComments;
      this.totalItems = pagedComments.totalCount;
    });
    this.pagingParametes = new PagingParameters(this.page, this.itemsPerPage);
    this.commentsService.getCommentsByBookId(this.bookId, this.pagingParametes);
  }

  getById() {
    this.bookId = this.route.snapshot.paramMap.get('bookId');
    this.bookService.getById(this.bookId).subscribe((data: BookViewModel) => {
      this.book = data;
    })
  }
  deleteBook(id: string) {
    this.bookService.delete(id).subscribe({

      next: (data: any) => {
        console.log('Book deleted successfully!');
        this.toastr.success('Book deleted successfully!');
        this.router.navigateByUrl('book/index');
      },
      error: (data: any) => {
        this.toastr.error(data);
      }
    })
  }
  makeUnavailable(id: string) {
    this.bookService.makeUnavailable(id).subscribe(response => {
      this.toastr.success('Book inactive');
      window.location.assign('/');
    }, error => {
      console.log(error);
      this.toastr.error('Availability unchanged')
    }
    )
  }
  checkRole(role: string) {
    this.role = this.authService.getRole();
    if (role == "administrator" || role == "librarian") {
      return true;
    }
    return false;
  }

  onReserve() {
    this.bookService.reserveBook(this.bookId).subscribe({
      next: () => {
        this.toastr.success('Book reserved successfully!');
        this.router.navigateByUrl('book-requests');
      },
      error: (errorMessage) => {
        this.toastr.error(errorMessage);
      }
    });

  }

  onOpenModal(content: any, bookId: string) {
    this.currentLoadedBookId = bookId;
    this.modalRef = this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onAddComment(modalForm: NgForm) {
    if (modalForm.invalid) {
      this.toastr.error('You cannot publish an empty comment');
      return;
    }

    const commentContent = modalForm.value['comment'];
    this.commentsService.addComment(commentContent, this.currentLoadedBookId).subscribe({
      next: _ => {
        this.toastr.success('Comment published successfully and is waiting for approval from a librarian');
        this.modalRef.close();
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });
  }

  onPageChange(nextPage: number) {
    this.page = nextPage;
    this.pagingParametes = new PagingParameters(this.page, this.itemsPerPage);
    this.commentsService.getCommentsByBookId(this.bookId, this.pagingParametes);
  }

  ngOnDestroy(): void {
    this.pagedCommentsSub.unsubscribe();
  }
}
