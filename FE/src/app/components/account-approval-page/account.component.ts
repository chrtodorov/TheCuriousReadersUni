import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountSerivce } from 'src/app/services/account.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { MailService } from 'src/app/services/mail.service';
import { UserViewModel } from 'src/app/models/user';

@Component({
  selector: 'app-account-approval-page',
  templateUrl: './account-approval-page.component.html',
  styleUrls: ['./account-approval-page.component.css']
})
export class AccountComponent implements OnInit {

  role: string;
  userId: string;
  users: UserViewModel[] = [];

  constructor(public accountService: AccountSerivce,
              private route: ActivatedRoute,
              private router: Router,
              private authService: AuthenticationService,
              private toastr: ToastrService,
              private formBuilder: FormBuilder,
              private mailService: MailService) { }

  mailForm = this.formBuilder.group({
  toEmail: [],
  subject: [],
  body: []
  })

  ngOnInit(): void {
    this.userId = this.route.snapshot.params['userId'];
    this.accountService.getPendingCustomers().subscribe((data: UserViewModel[])=>{
      this.users = data;
      this.role = this.authService.getRole()
    });
    }
    
    approve(id: string, emailAddress: string){
      let approveSubject = 'Your account has been approved';
      let approveBody = '<p>Hello,<br><br>We would like to inform you that your account was approved and successfully registered.<br><br>Enjoy your time in our website!<br><br>Greetings,<br>The Curious Readers Staff</p>'

      this.mailForm.patchValue({toEmail: emailAddress})
      this.mailForm.patchValue({subject: approveSubject})
      this.mailForm.patchValue({body: approveBody})

      this.mailService.sendEmail(this.mailForm.value).subscribe({
        next: () => {
          this.toastr.success('The customer has been informed that it\'s account was approved');
        },
        error: errorMessage => {
          this.toastr.error(errorMessage);
        }
      });
      this.accountService.approve(id).subscribe({
        next: (data: any) => {
          this.users = this.users.filter(data => data.userId !== id);
          this.toastr.success('Customer approved successfully!');
        },
        error: (data:any) => {
          this.toastr.error(data);
        }
      })
    }

    reject(id: string, emailAddress: string){
      let rejectSubject = 'Your account has been rejected';
      let rejectBody = '<p>Hello,<br><br>We would like to inform you that your account was rejected due to not meeting our requirements.<br><br>Please take your time to revise your registration.<br><br>Thank you in advance,<br>The Curious Readers Staff</p>'

      this.mailForm.patchValue({toEmail: emailAddress})
      this.mailForm.patchValue({subject: rejectSubject})
      this.mailForm.patchValue({body: rejectBody})
      
      this.mailService.sendEmail(this.mailForm.value).subscribe({
        next: () => {
          this.toastr.success('The customer has been informed that it\'s account was rejected');
        },
        error: errorMessage => {
          this.toastr.error(errorMessage);
        }
      });
      this.accountService.reject(id).subscribe({
        next: (data:any) => {
          this.users = this.users.filter(data => data.userId !== id);
          this.toastr.success('Customer rejected successfully!');
        },
        error: (data:any) => {

        }
      })
    }

  }
