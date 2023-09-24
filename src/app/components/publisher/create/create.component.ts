import { Component, OnInit } from '@angular/core';
import { PublisherService } from '../publisher.service'
import { FormGroup, FormControl, Validators} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

    form!: FormGroup;

    constructor(
      public publisherService: PublisherService,
      private toastr: ToastrService,
      public activeModal: NgbActiveModal) { }

    ngOnInit(): void {
      this.form = new FormGroup({
        name: new FormControl('', [Validators.required, Validators.maxLength(50)])
      })
    }

    get f() {
      return this.form.controls;
    }

    submit(){
    console.log(this.form.value);
    this.publisherService.create(this.form.value).subscribe({
      next: (data: any) => {
        console.log('Publisher created successfully!');
        this.toastr.success('Publisher created successfully!');
        this.activeModal.close();
      },
      error: (data: any) => {
        this.toastr.error(data);
      }      
    })
  }
  closeModal() {
    this.activeModal.close();
  }
}
