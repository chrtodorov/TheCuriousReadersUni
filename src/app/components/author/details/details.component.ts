import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Author } from '../author';
import { AuthorService } from '../author.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent implements OnInit {
  id: string = '';
  author: Author = new Author();

  constructor(
    public authorService: AuthorService,
    private route: ActivatedRoute
    ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['authorId'];

    this.authorService.getById(this.id).subscribe((data: Author) => {
      this.author = data;
    });
  } 
}