import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Author } from '../author';
import { AuthorService } from '../author.service';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent implements OnInit {
  id: string = '';
  author: Author = new Author();
  form!: FormGroup;

  constructor(
    public authorService: AuthorService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['authorId'];
    this.authorService.getById(this.id).subscribe((data: Author) =>{
      this.author = data;
    });

    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.maxLength(30)]),
      bio: new FormControl('', [Validators.maxLength(4000)])
    });
  }

  get f(){
    return this.form.controls;
  }

  submit(){
    console.log(this.form.value);
    this.authorService.update(this.id, this.form.value).subscribe({
      next: (data: any) => {
        console.log('Author updated successfully!');
        this.toastr.success('Author updated successfully!');
        this.router.navigateByUrl('author/index');
      },
      error: (data: any) => {
        this.toastr.error(data);
      }
    })
  }
}