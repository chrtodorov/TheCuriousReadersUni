import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Filters, Publisher, PublisherPagingParameters } from '../publisher';
import { PublisherService } from '../publisher.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit, OnDestroy {

  publishers: Publisher[] = [];
  public page = 1;
  public publishersPerPage = 10;
  public totalItems = 0;
  public filters: Filters = new Filters();
  public publisherPagingParameters = new PublisherPagingParameters(this.page, this.publishersPerPage);
  searchText = '';
  private publishersChangedSubscription: Subscription;
  private errorOccuredSubscription: Subscription;

  constructor(public publisherService: PublisherService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.publisherService.getAll(this.publisherPagingParameters);

    this.errorOccuredSubscription = this.publisherService.errorOccured.subscribe(error => {
      this.publishers = [];
    });

    this.publishersChangedSubscription = this.publisherService.publishersChanged.subscribe(publishers => {
      this.totalItems = this.publisherService.publishersCount;
      this.publishers = publishers;
    })
  }

  deletePublisher(id:string){
    this.publisherService.delete(id).subscribe({
      next: (data:any) => {
          this.publishers = this.publishers.filter(item => item.publisherId !== id);
          console.log('Publisher deleted successfully!');
          this.toastr.success('Publisher deleted successfully!');          
      },
      error: (data: any) => {
        this.toastr.error(data);
      }
    }
    )
  }
  onPageChange(nextPage: number){
    this.page = nextPage;
    this.publisherPagingParameters = new PublisherPagingParameters(this.page, this.publishersPerPage)
    this.getPublishers();
  }

  onChange(){
    this.filters.filter = this.searchText;
    this.filters.filterParameters = ['name'];
    if(!this.searchText){
      this.getPublishers();
    }
    if (this.searchText.length < 3) {
      return;
    }
    this.getPublishers();
  }

  private getPublishers(){
    if(!this.filters.filter){
      this.publisherService.getAll(this.publisherPagingParameters);
      return;
    }
    this.publisherService.getFiltered(
      this.filters.filterParameters,
      this.publisherPagingParameters,
      this.filters.filter
    )
  }

  ngOnDestroy(): void {
    this.publishersChangedSubscription.unsubscribe();
    this.errorOccuredSubscription.unsubscribe();
  }
}