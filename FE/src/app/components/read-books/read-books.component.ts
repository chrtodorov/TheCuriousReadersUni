import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';

import { PagingParameters } from 'src/app/models/book-parameter.model';
import { Book } from 'src/app/models/book.model';
import { BookService } from 'src/app/services/book.service';
import { CommentsService } from 'src/app/services/comments.service';

@Component({
  selector: 'app-read-books',
  templateUrl: './read-books.component.html',
  styleUrls: ['./read-books.component.css']
})
export class ReadBooksComponent implements OnInit, OnDestroy {
  public books: Book[];
  public itemsPerPage = 5;
  public page = 1;
  public totalItems: number;
  public pagingParametes: PagingParameters;
  private currentLoadedBookId: string;
  private modalRef: NgbModalRef;
  private pagedBooksChangedSub: Subscription;

  constructor(
    private bookService: BookService,
    private modalService: NgbModal,
    private commentsService: CommentsService,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.pagingParametes = new PagingParameters(this.page, this.itemsPerPage);
    this.bookService.getCurrentUserReadBooks(this.pagingParametes);
    this.pagedBooksChangedSub = this.bookService.pagedBooksChanged.subscribe({
      next: pagedBooks => {
        this.books = pagedBooks.data;
        this.totalItems = pagedBooks.totalCount;
      }
    })
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
    this.bookService.getCurrentUserReadBooks(this.pagingParametes);
  }

  ngOnDestroy(): void {
    this.pagedBooksChangedSub.unsubscribe();
  }

}
