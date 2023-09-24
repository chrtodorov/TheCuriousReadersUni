import { Component, OnInit } from '@angular/core';
import { PublisherService } from '../publisher.service';
import { Publisher } from '../publisher';
import { FormGroup, FormControl, Validators} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent implements OnInit {
  id: string = '';
  publisher: Publisher = new Publisher();
  form!: FormGroup;

  constructor(
    public publisherService: PublisherService, 
    private route: ActivatedRoute, 
    private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['publisherId'];
    this.publisherService.getById(this.id).subscribe((data: Publisher) =>{
      this.publisher = data;
    });

    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.maxLength(50)])
    });
  }

  get f(){
    return this.form.controls;
  }

  submit(){
    console.log(this.form.value);
    this.publisherService.update(this.id, this.form.value).subscribe({
      next: (data: any) => {
        console.log('Publisher updated successfully!');
        this.toastr.success('Publisher updated successfully!');
        this.router.navigateByUrl('publisher/index');
      },
      error: (data: any) =>{
        this.toastr.error(data);
      }
      })
  }
}
