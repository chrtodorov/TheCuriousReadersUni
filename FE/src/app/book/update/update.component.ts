import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { AuthorPagingParameters, Author } from 'src/app/components/author/author';
import { AuthorService } from 'src/app/components/author/author.service';
import { Filters,PublisherPagingParameters, Publisher } from 'src/app/components/publisher/publisher';
import { PublisherService } from 'src/app/components/publisher/publisher.service';
import { BookService } from 'src/app/services/book.service';
import { BlobMetadata } from '../../models/BlobMetadata';
import { BookCopy, BookViewModel, Genre } from '../../models/book';
import { CreateComponent as CreatePublisher } from  '../../components/publisher/create/create.component';
import { CreateComponent as CreateAuthor } from '../../components/author/create/create.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent implements OnInit {
  @ViewChild('auto') auto: any;
  
  blobId:string='';
  bookCopy:BookCopy[]=[];
  bookId:string = '';
  book:BookViewModel;
  id:string;
  keyword = 'name';
  publishers: Publisher[] = [];
  authors: Author[] = [];
  selectedAuthors: string[] = [];
  a:Author[]=[];
  selectedCopies: string[]=[];
  genres: Genre[] = [];

  public publisherFilters: Filters = new Filters();
  private publishersChangedSubscription: Subscription; 
  public publisherPagingParameters = new PublisherPagingParameters(1, 10);

  public authorFilters: Filters = new Filters();
  private authorsChangedSubscription: Subscription;
  public authorsPagingParameters = new AuthorPagingParameters(1, 10);

  form = this.fb.group({
    isbn: new FormControl('', [Validators.required, Validators.maxLength(17), Validators.pattern("((978[\--� ])?[0-9][0-9\--� ]{10}[\--� ][0-9xX])|((978)?[0-9]{9}[0-9Xx])")]),
    title: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(4000)]),
    genre: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    publisherId: new FormControl('', [Validators.required]),
    coverUrl: new FormControl(''),
    blobId: new FormControl(''),
    authorsIds: this.fb.array([], [Validators.required]),
    bookCopies: new FormArray([]),
});

  constructor(
    private router:Router, 
    private bookService:BookService,
    private authorService:AuthorService,
    private publisherService:PublisherService,
    private fb:FormBuilder,
    private route:ActivatedRoute,
    private toastr:ToastrService,
    private modalService: NgbModal
    ) { }

  ngOnInit(): void {
    this.getById();
    this.loadResources();
  }
  getById(){
    this.bookId = this.route.snapshot.paramMap.get('bookId');
    this.bookService.getById(this.bookId).subscribe((data: BookViewModel)=>{
      this.book = data;
      this.a = data.authors
      this.bookCopy = data.bookCopies
      this.loadCopy();
      this.loadAuthor();
      this.loadBlob();
    })
  }

  get f(){
    return this.form.controls;
  }

  submit(){
    console.log(this.form.value)

    this.bookService.update(this.book.bookId,this.form.value).subscribe({
      next:(data:any)=>{
        console.log('Book updated successfully')
        this.toastr.success('Book updated successfully')
        this.router.navigateByUrl('books');
      },
      error: (data:any) => {
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
  loadCopy(){
    for(let copy of this.bookCopy){
      const bookCopyForm = this.fb.group({
        barcode: [copy.barcode, [Validators.required, Validators.maxLength(10)]]
      });
      this.bookCopies.push(bookCopyForm);
    }
  }

  deleteCopy(copyIndex:number){
    this.bookCopy.splice(copyIndex,1)
    this.selectedCopies.splice(copyIndex,1)
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
  createPublisher(){
    const modalRef = this.modalService.open(CreatePublisher)
    modalRef.componentInstance.name = "CreatePublisherModal"
  }

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
    if(!val){
      this.getPublishers();
    }
    if (val.length < 3) {
      return;
    }
    this.getPublishers();
  }
  
  onFocusedPublisher(e: any){
    this.getPublishers();
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

  loadAuthor(){
    for(let author of this.a){
      this.selectedAuthors.push(author.name)
      this.authorsIds.push(new FormControl (author.authorId))
    }
  }

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

  onChangeSearchAuthor(val: string) {
    this.authorFilters.filter = val;
    this.authorFilters.filterParameters = ['name'];
    if(!val){
      this.getAuthors();
    }
    if (val.length < 3) {
      return;
    }
    this.getAuthors();
  }
  
  onFocusedAuthor(e: any){
    this.getAuthors();
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
  createAuthor(){
    const modalRef = this.modalService.open(CreateAuthor)
    modalRef.componentInstance.name = "CreateAuthorModal"
  }
  onClearAuthor() {
    this.authorFilters.filter = '';
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

  loadBlob(){
    this.form.patchValue({coverUrl:this.book.coverUrl, blobId:this.book.blobId})
  }

  onBlobChange(blob: BlobMetadata){
    this.form.patchValue({ coverUrl: blob.url, blobId: blob.id })
  }

  selectEventGenre(item: any){
    if(typeof(item) === 'string')
    {
      this.form.patchValue({genre: item})
    }
    else
    { 
      this.form.patchValue({genre: item.name})
    }
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
