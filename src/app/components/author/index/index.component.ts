import { Component, OnDestroy, OnInit} from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Filters } from '../author';
import { Author, AuthorPagingParameters } from '../author';
import { AuthorService } from '../author.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit, OnDestroy {
  public authors: Author[] = [];
  public page = 1;
  public authorsPerPage = 10;
  public totalItems = 0;
  public filters: Filters = new Filters();
  public authorPagingParameters = new AuthorPagingParameters(this.page, this.authorsPerPage);
  public searchText: '';
  private authorsChangedSubscription: Subscription;
  private errorOccuredSubscription: Subscription;

  constructor(public authorService: AuthorService, private toastr: ToastrService) { }
  

  ngOnInit(): void {
    this.authorService.getAll(this.authorPagingParameters);

    this.errorOccuredSubscription = this.authorService.errorOccured.subscribe(error => {
      this.authors = [];
    });

    this.authorsChangedSubscription = this.authorService.authorsChanged.subscribe(authors => {
      this.totalItems = this.authorService.authorsCount;
      this.authors = authors;
    })
  }

  deleteAuthor(id:string){
    this.authorService.delete(id).subscribe({

    next:(data: any) => {
    this.authors = this.authors.filter(item => item.authorId !== id);
    console.log('Author deleted successfully!');
    this.toastr.success('Author deleted successfully!');
    },
    error: (data: any) => {
      this.toastr.error(data);
    }
    })
  }

  onPageChange(nextPage: number){
    this.page = nextPage;
    this.authorPagingParameters = new AuthorPagingParameters(this.page, this.authorsPerPage)
    this.getAuthors();
  }

  onChange(){
    this.filters.filter = this.searchText;
    this.filters.filterParameters = ['name'];
    if(!this.searchText){
      this.getAuthors();
    }
    if (this.searchText.length < 3) {
      return;
    }
    this.getAuthors();
  }

  private getAuthors(){
    if(!this.filters.filter){
      this.authorService.getAll(this.authorPagingParameters);
      return;
    }
    this.authorService.getFiltered(
      this.filters.filterParameters,
      this.authorPagingParameters,
      this.filters.filter
    )
  }

  ngOnDestroy(): void {
    this.authorsChangedSubscription.unsubscribe();
    this.errorOccuredSubscription.unsubscribe();
  }
}