import { Component, OnInit } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { Book } from '../models/book.model';
import { BookService } from '../services/book.service';
import { UserService } from '../services/user.service';


@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit {

  books: Book[];
  count:number;
  bookCount:number;
  genreCount:number;
  p:number;
  constructor(private bookService:BookService, public router:Router, private userService:UserService) { }

  ngOnInit(): void {
  this.getNewReleases();
  this.getBookCount();
  this.getUserCount();
  this.getGenreCount();
  }

  getNewReleases(){
    this.bookService.getLatest().subscribe(response=>(
      this.books=response
    ));
    return this.books;
  }
  getBookCount(){
    this.bookService.getCount().subscribe((data:any)=>
    this.bookCount = data
    )
    return this.bookCount;
  }

  getUserCount(){
    this.userService.getUserCount().subscribe((data:any)=>
    this.count = data
    )
    return this.count;
  }

  getGenreCount(){
    this.bookService.getGenreCount().subscribe((data:any)=>
    this.genreCount = data
    )
    return this.bookCount;
  }
}
