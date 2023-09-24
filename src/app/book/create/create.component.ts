import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Author, AuthorPagingParameters } from 'src/app/components/author/author';
import { Filters, Publisher, PublisherPagingParameters } from 'src/app/components/publisher/publisher';
import { BookService } from '../../services/book.service';
import { AuthorService } from 'src/app/components/author/author.service';
import { PublisherService } from 'src/app/components/publisher/publisher.service';
import { Subscription } from 'rxjs';
import { BlobMetadata } from '../../models/BlobMetadata';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateComponent as CreatePublisher } from  '../../components/publisher/create/create.component';
import { CreateComponent as CreateAuthor } from '../../components/author/create/create.component';
import { Genre } from '../../models/book';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {
  @ViewChild('auto') auto: any;
  
  public publisherFilters: Filters = new Filters();
  private publishersChangedSubscription: Subscription; 
  public publisherPagingParameters = new PublisherPagingParameters(1, 10);

  public authorFilters: Filters = new Filters();
  private authorsChangedSubscription: Subscription;
  public authorsPagingParameters = new AuthorPagingParameters(1, 10);

  keyword = 'name';
  publishers: Publisher[] = [];
  authors: Author[] = [];
  genres: Genre[] = [];
  selectedAuthors: string[] = [];

  form = this.fb.group({
      isbn: new FormControl('', [Validators.required, Validators.maxLength(17), Validators.pattern("((978[\--� ])?[0-9][0-9\--� ]{10}[\--� ][0-9xX])|((978)?[0-9]{9}[0-9Xx])")]),
      title: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      description: new FormControl('', [Validators.required, Validators.maxLength(4000)]),
      genre: new FormControl('', [Validators.required, Validators.maxLength(20)]),
      publisherId: new FormControl('', [Validators.required]),
      coverUrl: new FormControl('', [Validators.required]),
      blobId: new FormControl(''),
      authorsIds: this.fb.array([], Validators.required),
      bookCopies: this.fb.array([], Validators.required)
  });
 

  constructor(
    public bookService: BookService,
    public router: Router,
    private toastr: ToastrService,
    public authorService: AuthorService, 
    public publisherService: PublisherService,
    private fb:FormBuilder,
    private modalService: NgbModal
    ) { }

  ngOnInit(): void {  
    this.loadResources();
  }

  get f(){
    return this.form.controls;
  }

  submit(){
    console.log(this.form.value);
    this.bookService.create(this.form.value).subscribe({
      next: (data: any) => {
        console.log('Book create successfully!');
        this.toastr.success('Book create successfully!');
        this.router.navigateByUrl('book/index');
      },
      error: (data: any) => {
        this.toastr.error(data);
      }
    })
  }

  addBookCopy() {
    const bookCopyForm = this.fb.group({
      barcode: ['', [Validators.required, Validators.maxLength(10)]]
    });
    this.bookCopies.push(bookCopyForm);
  }

  deleteBookCopy(bookCopyIndex: number) {
    this.bookCopies.removeAt(bookCopyIndex);
  }

  get bookCopies() {
    return this.form.controls["bookCopies"] as FormArray;
  }
  get authorsIds() {
    return this.form.controls["authorsIds"] as FormArray;
  }

  // Publishers AutoComplete Methods
  selectEventPublisher(item: Publisher) {
    this.form.patchValue({ publisherId: item.publisherId})
  }
  onClearPublisher() {
    this.form.patchValue({ publisher: ''});
    this.publisherFilters.filter = '';
  }

  onChangeSearchPublisher(val: string) {
    this.publisherFilters.filter = val;
    this.publisherFilters.filterParameters = ['name'];
    this.getPublishers();
  }
  
  onFocusedPublisher(e: any){
    this.getPublishers();
  }

  createPublisher(){
    const modalRef = this.modalService.open(CreatePublisher)
    modalRef.componentInstance.name = "CreatePublisherModal"
  }

  private getPublishers(){
    if(!this.publisherFilters.filter){
      this.publisherService.getAll(this.publisherPagingParameters);
      return;
    }
    this.publisherService.getFiltered(
      this.publisherFilters.filterParameters,
      this.publisherPagingParameters,
      this.publisherFilters.filter
    )
  }

  // Author AutoComplete logic
  selectEventAuthor(item: Author) {
    for (const id of this.authorsIds.value) {
      if(id === item.authorId){
        this.toastr.error("This autor is already added.")
        return;
      }
    }
    
    this.selectedAuthors.push(item.name);
    this.authorsIds.push(new FormControl(item.authorId));
    this.auto.clear();
  }
  onClearAuthor() {
    this.authorFilters.filter = '';
  }

  onChangeSearchAuthor(val: string) {
    this.authorFilters.filter = val;
    this.authorFilters.filterParameters = ['name'];
    this.getAuthors();
  }
  
  onFocusedAuthor(e: any){
    this.getAuthors();
  }

  createAuthor(){
    const modalRef = this.modalService.open(CreateAuthor)
    modalRef.componentInstance.name = "CreateAuthorModal"
  }

  private getAuthors(){
    if(!this.authorFilters.filter){
      this.authorService.getAll(this.authorsPagingParameters);
      return;
    }
    this.authorService.getFiltered(
      this.authorFilters.filterParameters,
      this.authorsPagingParameters,
      this.authorFilters.filter
    )
  }

  clearSelectedAuthors(){
    this.authorsIds.clear();
    this.selectedAuthors = [];
  }

  loadResources(){
    this.publishersChangedSubscription = this.publisherService.publishersChanged.subscribe(publishers => {
      this.publishers = publishers;
    })
    this.authorsChangedSubscription = this.authorService.authorsChanged.subscribe(authors => {
      this.authors = authors;
    })
    this.bookService.getGenres().subscribe((data: Genre[]) => {
      this.genres = data;
    })
  }

  onBlobChange(blob: BlobMetadata){
    this.form.patchValue({ coverUrl: blob.url, blobId: blob.id })
  }

  selectEventGenre(item: Genre){
    this.form.patchValue({genre: item.name})
  }
  onInputChangeGenre(item: string){
    this.form.patchValue({genre: item})
  }
  onClearGenre(){
    this.form.patchValue({genre: ''})
  }
  

  ngOnDestroy(): void {
    this.publishersChangedSubscription.unsubscribe();
    this.authorsChangedSubscription.unsubscribe();
  }
}
