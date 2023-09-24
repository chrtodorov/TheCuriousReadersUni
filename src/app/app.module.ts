import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { ToastrModule, ToastrService } from 'ngx-toastr';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/authentication/login/login.component';
import { AuthenticationService } from './services/authentication.service';
import { RouterModule } from '@angular/router';
import { AccountComponent } from './components/account-approval-page/account.component';
import { JwtInterceptor } from './helpers/jwt.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TooltipModule } from 'ngx-bootstrap/tooltip'
import { PublisherModule } from './components/publisher/publisher.module';
import { AuthorModule } from './components/author/author.module';
import { BookModule } from './book/book.module';
import { AuthGuardService } from './helpers/auth.guard';
import { NgxPaginationModule } from 'ngx-pagination'
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { BookHeaderComponent } from './components/book-list/book-header/book-header.component';
import { BookListComponent } from './components/book-list/book-list.component';
import { HeaderComponent } from './components/header/header.component';
import { RegisterComponent } from './components/authentication/register/register.component';
import { BookRequestsComponent } from './components/book-requests/book-requests.component';
import { ProfileDetailsComponent } from './components/authentication/profile-details/profile-details.component';
import { BookLoansComponent } from './components/book-loans/book-loans.component';
import { DateValidatorDirective } from './directives/date-validator.directive';
import { BookLoanItemComponent } from './components/book-loans/book-loan-item/book-loan-item.component';

import { HomepageComponent } from './homepage/homepage.component';
import { ReadBooksComponent } from './components/read-books/read-books.component';
import { PendingCommentsComponent } from './components/pending-comments/pending-comments.component';

import { CommonModule } from '@angular/common';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    AccountComponent,
    HeaderComponent,
    BookListComponent,
    BookHeaderComponent,
    RegisterComponent,
    BookRequestsComponent,
    ProfileDetailsComponent,
    BookRequestsComponent,
    BookLoansComponent,
    DateValidatorDirective,
    BookLoanItemComponent,
    HomepageComponent,
    ReadBooksComponent,
    PendingCommentsComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule,
    AppRoutingModule,
    PublisherModule,
    HttpClientModule,
    AuthorModule,
    BookModule,
    BrowserAnimationsModule,
    NgMultiSelectDropDownModule,
    NgbDatepickerModule,
    NgxPaginationModule,
    NgbModule,
    ToastrModule.forRoot(),
    TooltipModule.forRoot(),
    RouterModule.forChild([
      { path: 'login', component: LoginComponent }
    ]),
    BrowserAnimationsModule
  ],
  providers: [AuthenticationService, ToastrService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },

    [AuthGuardService]
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
