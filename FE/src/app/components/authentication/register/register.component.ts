import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ConfirmedValidator } from 'src/app/helpers/customerValidators';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Login } from 'src/app/models/login';
import { User } from 'src/app/models/user';
import countries from 'src/app/helpers/countries.json';
import { throwError } from 'rxjs';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model: User = new User();
  role: string;
  passwordRegex = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#])[A-Za-z\\d@$!%*?&#]{10,65}$';
  emailRegex = '^(([^<>()[\\]\\\\.,;:\\s@\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\"]+)*)|(\".+\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$'
  phoneNumberRegex = '\\+[0-9]+';
  nameRegex = "([a-zA-Z',.-]+( [a-zA-Z',.-]+)*)";
  lettersAndNumersRegex = "([a-zA-Z0-9',.-]+( [a-zA-Z0-9',.-]+)*)";
  countryList:{name:string}[] = countries;

  constructor(public authService: AuthenticationService, 
              private router: Router, 
              private toastr: ToastrService,
              private formBuilder: FormBuilder) { }

  ngOnInit(): void {this.setRole()}

  registerForm = this.formBuilder.group({
    firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(128), Validators.pattern(this.nameRegex)]],
    lastName:  ['', [Validators.required, Validators.minLength(2), Validators.maxLength(128), Validators.pattern(this.nameRegex)]],
    emailAddress: ['', [Validators.required, Validators.pattern(this.emailRegex)]],
    password: ['', [Validators.required, Validators.pattern(this.passwordRegex)]],
    confirmPassword: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.phoneNumberRegex)]],
    roleName: ['customer', [Validators.required]],
    address: this.formBuilder.group({
      country: ['', [Validators.required]],
      city: ['', [Validators.required, Validators.maxLength(128), Validators.pattern(this.lettersAndNumersRegex)]],
      street: ['', [Validators.required, Validators.pattern(this.lettersAndNumersRegex)]],
      streetNumber: ['', [Validators.required, Validators.pattern(this.lettersAndNumersRegex)]],
      buildingNumber: ['', [Validators.maxLength(65), Validators.pattern(this.lettersAndNumersRegex)]],
      apartmentNumber: ['', [Validators.maxLength(65), Validators.pattern(this.lettersAndNumersRegex)]],
      additionalInfo: ['', [Validators.maxLength(1028)]],
    })
  }, {
    validator: ConfirmedValidator('password','confirmPassword')
  })

  registerLibrarianForm = this.formBuilder.group({
    firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(128), Validators.pattern(this.nameRegex)]],
    lastName:  ['', [Validators.required, Validators.minLength(2), Validators.maxLength(128), Validators.pattern(this.nameRegex)]],
    emailAddress: ['', [Validators.required,Validators.pattern(this.emailRegex)]],
    password: ['', [Validators.required, Validators.pattern(this.passwordRegex)]],
    confirmPassword: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.phoneNumberRegex)]],
    roleName: ['librarian', [Validators.required]],
    address: this.formBuilder.group({
      country: ['', [Validators.required]],
      city: ['', [Validators.required, Validators.maxLength(128), Validators.pattern(this.lettersAndNumersRegex)]],
      street: ['', [Validators.required, Validators.pattern(this.lettersAndNumersRegex)]],
      streetNumber: ['', [Validators.required, Validators.pattern(this.lettersAndNumersRegex)]],
      buildingNumber: ['', [Validators.maxLength(65), Validators.pattern(this.lettersAndNumersRegex)]],
      apartmentNumber: ['', [Validators.maxLength(65), Validators.pattern(this.lettersAndNumersRegex)]],
      additionalInfo: ['', [Validators.maxLength(1028)]],
    })
  }, {
    validator: ConfirmedValidator('password','confirmPassword')
  })


  get f(){
    return this.registerForm.controls;
  }

  get address(){
    return ((this.registerForm.get('address') as FormGroup).controls)
  }

  get librarian(){
    return this.registerLibrarianForm.controls;
  }
  get librarianAddress(){
    return ((this.registerLibrarianForm.get('address') as FormGroup).controls)
  }
  
  setRole(){
    const role: Login = JSON.parse(sessionStorage.getItem('role'));
    this.role = this.authService.getRole();
    return role;
  }


  register(){
    this.authService.register(this.registerForm.value).subscribe({
      next: (data: any) => {
        this.router.navigateByUrl('');
        this.toastr.success('Your account has been registered. You will receive an email when your account is approved and ready to be used.');
    }, 
    error: (data: any) => {
      this.toastr.error(data.error.message);
    }
    })
  }

  registerLibrarian(){
    this.authService.register(this.registerLibrarianForm.value).subscribe({
      next: (data: any) => {
        this.router.navigateByUrl('');
        this.toastr.success('A Librarian has been registered successfully!');
    }, 
    error: (data: any) => {
      this.toastr.error(data.error.message);
    }
    })
  }

  

}
