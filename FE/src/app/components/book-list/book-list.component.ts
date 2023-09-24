import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';
import { PagingParameters } from '../../models/book-parameter.model';
import { Filters } from '../../models/filters.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit, OnDestroy {
  public books: Book[] = [];
  public page = 1;
  public booksPerPage = 10;
  public totalItems = 0;
  public filters: Filters = new Filters();
  public bookPagingParameters = new PagingParameters(this.page, this.booksPerPage);
  private booksChangedSubscription: Subscription;
  private errorOccuredSubscription: Subscription;

  constructor(
    private bookService: BookService, 
    private route: ActivatedRoute, 
    private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      let pageParam = params['page'];
      this.page = pageParam ? +pageParam : 1;
      this.bookPagingParameters = new PagingParameters(this.page, this.booksPerPage);
    });

    this.bookService.getBooks(this.bookPagingParameters);

    this.errorOccuredSubscription = this.bookService.errorOccured.subscribe(error => {
      this.books = [];
    });

    this.booksChangedSubscription = this.bookService.booksChanged.subscribe(books => {
      this.totalItems = this.bookService.booksCount;
      this.books = books;
    });
  }

  onFiltersChanged(value: any) {
    this.filters = value;
    this.getBooks();
  }

  onPageChange(nextPage: number) {    
    this.page = nextPage;
    this.bookPagingParameters = new PagingParameters(this.page, this.booksPerPage);
    this.getBooks();
    this.setQueryParams();
  }

  private setQueryParams() {
    this.page == 1 ?
      this.router.navigate([]) :
      this.router.navigate([], { relativeTo: this.route, queryParams: { page: this.page } });
  }

  private getBooks() {
    if (!this.filters.filter) {
      this.bookService.getBooks(this.bookPagingParameters);
      return;
    }
    this.bookService.getFilteredBooks(
      this.filters.filterParameters, 
      this.bookPagingParameters, 
      this.filters.filter);
  }

  ngOnDestroy(): void {
    this.booksChangedSubscription.unsubscribe();
    this.errorOccuredSubscription.unsubscribe();
  }
}
