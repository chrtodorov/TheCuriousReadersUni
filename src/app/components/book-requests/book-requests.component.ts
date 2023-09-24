import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, NgForm } from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { BookRequestModified } from '../../models/book-request.model';
import { BookService } from '../../services/book.service';
import { PagingParameters } from 'src/app/models/book-parameter.model';
import { Subscription } from 'rxjs';
import { BookLoanRequest } from 'src/app/models/book-loan-request.model';
import { PagedRequest } from 'src/app/models/paged-request.model';
import { MailService } from 'src/app/services/mail.service';

@Component({
  selector: 'app-book-requests',
  templateUrl: './book-requests.component.html',
  styleUrls: ['./book-requests.component.css']
})
export class BookRequestsComponent implements OnInit, OnDestroy {
  public userRole: string;
  public bookRequests: PagedRequest<BookRequestModified>;
  public requestsPerPage = 1;
  public page = 1;
  public totalItems: number;
  public bookPagingParameters: PagingParameters;
  private bookRequestsChangedSubscription: Subscription;
  private getRequests: Function;
  private modalFormIsValid = false;
  private modalRef: NgbModalRef;

  constructor(
    private booksService: BookService,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private mailService: MailService) { }

    mailForm = this.formBuilder.group({
    toEmail: [],
    subject: ['Book Request Rejected'],
    body: []
  })

  ngOnInit(): void {
    this.bookRequestsChangedSubscription = this.booksService.bookRequestsChanged.subscribe(bookRequests => {
      this.bookRequests = bookRequests;      
      this.totalItems = bookRequests.totalCount;
    });

    this.route.queryParams.subscribe(params => {
      let pageParam = params['page'];
      this.page = pageParam ? +pageParam : 1;
      this.bookPagingParameters = new PagingParameters(this.page, this.requestsPerPage);
    });

    this.userRole = JSON.parse(sessionStorage.getItem('role'));
    if (this.userRole === 'customer') {
      this.getRequests = () => this.booksService.getCurrentUsersBookRequests(this.bookPagingParameters);
    }
    else if (this.userRole === 'librarian') {
      this.getRequests = () => this.booksService.getBookRequests(this.bookPagingParameters);
    }
    else {
      this.router.navigate(['/']);
    }
    this.getRequests();
  }

  onPageChange(nextPage: number) {
    this.page = nextPage;
    this.bookPagingParameters = new PagingParameters(this.page, this.requestsPerPage);
    this.getRequests();
    this.setQueryParams();
  }

  onApprove(content: any, emailAddress: string) {
    let approveSubject = 'Your book request was approved'
    let approveBody = '<p>Hello,<br><br>We would like to inform you that your book request was approved.<br><br>Enjoy your time reading the book!<br><br>Greetings,<br>The Curious Readers Staff</p>'

    this.mailForm.patchValue({toEmail: emailAddress});
    this.mailForm.patchValue({subject: approveSubject});
    this.mailForm.patchValue({body: approveBody})
    this.mailService.sendEmail(this.mailForm.value).subscribe({
      next: () => {
        this.toastr.success('The customer has been informed that the book request was approved.')
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });
    this.modalRef = this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onReject(bookRequestId: string, emailAddress: string) {
    let rejectSubject = 'Your book request was rejected'
    let rejectBody = '<p>Hello,<br><br>We would like to inform you that your book request was rejected due to internal politics.<br><br>Please try again in a few days.<br><br>Thank you,<br>The Curious Readers Staff</p>'

    this.mailForm.patchValue({toEmail: emailAddress});
    this.mailForm.patchValue({subject: rejectSubject});
    this.mailForm.patchValue({body: rejectBody})

    this.mailService.sendEmail(this.mailForm.value).subscribe({
      next: () => {
        this.toastr.success('The customer has been informed that the book request was rejected.')
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });
    this.booksService.rejectRequest(bookRequestId).subscribe({
      next: () => {
        this.toastr.success('Book request rejected successfully');
        if (this.page > 1) {
          this.onPageChange(this.page - 1);
        }
        else {
          this.getRequests();
        }
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    })
  }

  onMakeLoan(modalForm: NgForm) {
    if (!modalForm.touched || modalForm.invalid || !modalForm.value.from || !modalForm.value.to) {
      return;
    }
    const from = new Date(modalForm.value.from.year, modalForm.value.from.month - 1, modalForm.value.from.day, 12);
    const to = new Date(modalForm.value.to.year, modalForm.value.to.month - 1, modalForm.value.to.day, 12);
    if (from >= to) {
      return;
    }
    const customerId = this.bookRequests.data[0]?.requestedById;
    const bookCopyId = this.bookRequests.data[0]?.bookCopyId;
    this.modalFormIsValid = true;

    this.booksService.makeLoan(new BookLoanRequest(from, to, bookCopyId, customerId))
      .subscribe({
        next: loan => {
          this.toastr.success('Book loaned successfully');
          if (this.page > 1) {
            this.onPageChange(this.page - 1);
          }
          else {
            this.getRequests();
          }
        },
        error: errorMessage => {
          this.toastr.error(errorMessage);
          this.modalService.dismissAll(errorMessage);
        }
      });
    if (this.modalFormIsValid) {
      this.modalRef.close()
    }
  }

  private setQueryParams() {
    this.page == 1 ?
      this.router.navigate([]) :
      this.router.navigate([], { relativeTo: this.route, queryParams: { page: this.page } });
  }

  ngOnDestroy(): void {
    this.bookRequestsChangedSubscription.unsubscribe();
  }
}
