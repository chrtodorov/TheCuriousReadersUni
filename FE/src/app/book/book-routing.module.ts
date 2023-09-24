import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BookListComponent } from '../components/book-list/book-list.component';
import { AuthGuardService } from '../helpers/auth.guard';
import { BookdetailsComponent } from './bookdetails/bookdetails.component';
import { CreateComponent } from './create/create.component';
import { UpdateComponent } from './update/update.component';
import { BookRequestsComponent } from '../components/book-requests/book-requests.component';
import { BookLoansComponent } from '../components/book-loans/book-loans.component';
import { BookLoanItemComponent } from '../components/book-loans/book-loan-item/book-loan-item.component';
import { ReadBooksComponent } from '../components/read-books/read-books.component';
import { CustomerGuardService } from '../helpers/customer.guard';

const routes: Routes = [
  { path: 'book', redirectTo: 'book/index', pathMatch: 'full' },
  { path: 'book/index', component: BookListComponent },
  { path: 'book/:bookId/details', component: BookdetailsComponent },
  { path: 'book/create', component: CreateComponent, canActivate: [AuthGuardService] },
  { path: 'book/:bookId/update', component: UpdateComponent, canActivate: [AuthGuardService] },
  { path: 'books', component: BookListComponent },
  { path: 'book-requests', component: BookRequestsComponent },
  { path: 'book-loans', component: BookLoansComponent },
  { path: 'book-loans/:userId', component: BookLoanItemComponent },
  { path: 'books/read', component: ReadBooksComponent, canActivate: [CustomerGuardService] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookRoutingModule { }
