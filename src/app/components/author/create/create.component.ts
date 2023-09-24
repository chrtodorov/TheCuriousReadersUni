import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { AuthorService } from '../author.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  form!: FormGroup;

  constructor(
    public authorService: AuthorService,
    private toastr: ToastrService,
    public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
    this.form = new FormGroup ({
      name: new FormControl('', [Validators.required, Validators.maxLength(30)]),
      bio: new FormControl('', [Validators.maxLength(4000)])
    })
  }

  get f() {
    return this.form.controls;
  }

  submit(){
    console.log(this.form.value);
    this.authorService.create(this.form.value).subscribe({
      next: (data:any) => {
      console.log('Author created successfully!');
      this.toastr.success('Author created successfully!');
      this.activeModal.close();
    },
    error: (data: any) => {
      this.toastr.error(data);
    }
    })
  }
  closeModal(){
    this.activeModal.close();
  }
}
