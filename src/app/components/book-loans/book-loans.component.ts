import { Component, ElementRef, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';

import { PagingParameters } from '../../models/book-parameter.model';
import { BookService } from '../../services/book.service';
import { BookLoan } from '../../models/book-loan.model';
import { ToastrService } from 'ngx-toastr';
import { ProlongRequest } from 'src/app/models/prolong-request.model';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/models/user.model';

@Component({
  selector: 'app-book-loans',
  templateUrl: './book-loans.component.html',
  styleUrls: ['./book-loans.component.css']
})
export class BookLoansComponent implements OnInit, OnDestroy {
  public userRole: string;
  public bookLoans: BookLoan[];
  public page = 1;
  public totalItems: number;
  public loansPerPage = 1;
  public loadingAll = true;
  public hasAnyLoans = true;
  public users: User[];
  public currentLoanUser: User;
  public bookPagingParameters = new PagingParameters(this.page, this.loansPerPage);
  private bookLoansChangedSub: Subscription;
  private userModalRef: NgbModalRef;
  private prolongModalRef: NgbModalRef;
  private currentBookLoanId: string;
  private type: 'all' | 'pending' | 'user' = 'all';
  @ViewChild('content') private userTemplateRef: TemplateRef<any>;
  @ViewChild('all') private allCheckbox: ElementRef;
  @ViewChild('user') private userCheckbox: ElementRef;

  constructor(
    private booksService: BookService,
    private usersService: UserService,
    private modalService: NgbModal,
    private toastr: ToastrService,) { }

  ngOnInit(): void {
    this.bookLoansChangedSub = this.booksService.bookLoansChanged.subscribe(loans => {
      this.bookLoans = loans.data;
      this.totalItems = loans.totalCount;
      if (this.loadingAll && this.totalItems === 0) {
        this.hasAnyLoans = false;
      }
    });

    this.userRole = JSON.parse(sessionStorage.getItem('role'));
    if (this.userRole === 'customer') {
      this.booksService.getCurrentUsersLoans(this.bookPagingParameters);
    }
    else if (this.userRole === 'librarian') {
      this.loadingAll = true;
      this.booksService.getAllLoans(this.bookPagingParameters);
    }
    this.bookPagingParameters = new PagingParameters(this.page, this.loansPerPage);
  }

  onSelect(type: 'all' | 'pending' | 'user', completedLoan = false) {
    this.type = type;
    if (this.hasAnyLoans) {
      if (this.type === 'user' && !completedLoan) {
        this.userModalRef = this.modalService.open(this.userTemplateRef, { ariaLabelledBy: 'modal-basic-title' });
        this.handleModalDropdownClick();
        this.userModalRef.dismissed.subscribe(() => {
          this.onDismiss(this.allCheckbox.nativeElement, this.userCheckbox.nativeElement);
        });
      }
      else {
        this.onPageChange(1);
      }
    }
  }

  onGetByUser(modalForm: NgForm) {
    if (modalForm.invalid) {
      return;
    }
    this.booksService.userBookLoansChanged.subscribe({
      next: loans => {
        this.bookLoans = loans.data;
        this.totalItems = loans.totalCount;
        this.userModalRef.close();
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      },
    });
    this.onPageChange(1);
  }

  onPageChange(nextPage: number) {
    this.page = nextPage;
    this.bookPagingParameters = new PagingParameters(this.page, this.loansPerPage);
    this.loadingAll = false;

    if (this.userRole === 'librarian') {
      if (this.type === 'user') {
        this.booksService.getLoansByUserId(this.currentLoanUser.userId, this.bookPagingParameters);
      }
      else if (this.type === 'pending') {
        this.booksService.getExpiringLoans(this.bookPagingParameters);
      }
      else {
        this.loadingAll = true;
        this.booksService.getAllLoans(this.bookPagingParameters);
      }
    }
    else {
      this.booksService.getCurrentUsersLoans(this.bookPagingParameters);
    }
  }

  onOpenProlongModal(bookLoanId: string, content: any) {
    this.currentBookLoanId = bookLoanId;
    this.prolongModalRef = this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onProlong(prolongForm: NgForm) {
    if (prolongForm.invalid) {
      return;
    }
    const prolongRequestToString = prolongForm.value['prolongTo'];
    const prolongRequest = new Date(prolongRequestToString.year, prolongRequestToString.month - 1, prolongRequestToString.day, 12);

    this.booksService.prolongLoan(this.currentBookLoanId, new ProlongRequest(prolongRequest)).subscribe({
      next: _ => {
        this.toastr.success("Book loan prolonged successfully");
        this.prolongModalRef.close();
        this.onSelect(this.type);
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    });
  }

  onGetUsers(userInput: HTMLInputElement, menu: HTMLDivElement) {
    this.usersService.getUsers(userInput.value).subscribe(users => {
      this.users = users.filter(u => u.emailAddress !== userInput.value);
      menu.classList.toggle('show');
    });

  }

  onChosenUser(user: User, userInput: HTMLInputElement, menu: HTMLDivElement) {
    this.currentLoanUser = user;
    userInput.value = user.emailAddress;
    menu.classList.toggle('show');
  }

  onCompleteLoan(loan: BookLoan) {
    this.booksService.completeLoan(loan.bookLoanId, loan.book.bookId, loan.loanedTo.userId).subscribe({
      next: () => {
        this.toastr.success('Loan completed successfully');
        this.onSelect(this.type, true);
      },
      error: errorMessage => {
        this.toastr.error(errorMessage);
      }
    })
  }

  onDismiss(allCheckBox: HTMLInputElement, userCheckBox: HTMLInputElement) {
    userCheckBox.blur();
    allCheckBox.checked = true;
    allCheckBox.focus();
    this.booksService.getAllLoans(this.bookPagingParameters);
  }

  handleModalDropdownClick() {
    const modal = document.getElementsByClassName('modal')[0] as HTMLElement;
    if (modal) {
      modal.onclick = (e) => {
        if (!(<HTMLElement>e.target).classList.contains('dropdown-item')) {
          let dropdowns = document.getElementsByClassName('dropdown-content');
          let i;
          for (i = 0; i < dropdowns.length; i++) {
            let openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
              openDropdown.classList.remove('show');
            }
          }
        }
      }
    }
  }

  ngOnDestroy(): void {
    this.bookLoansChangedSub.unsubscribe();
  }
}
