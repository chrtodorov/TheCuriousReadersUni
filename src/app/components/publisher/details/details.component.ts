import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Publisher } from '../publisher';
import { PublisherService } from '../publisher.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent implements OnInit {

  id: string = '';
  publisher: Publisher = new Publisher();

  constructor(
    public publisherService: PublisherService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['publisherId'];

    this.publisherService.getById(this.id).subscribe((data: Publisher) => {
      this.publisher = data;
    });
  }
}
