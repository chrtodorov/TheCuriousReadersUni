import { Component, OnInit } from '@angular/core';
import {  UserViewModel } from 'src/app/models/user';
import { ProfileService } from 'src/app/services/profile.service';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.html',
  styleUrls: ['./profile-details.css']
})
export class ProfileDetailsComponent implements OnInit {

  userId: string = '';
  user: UserViewModel = new UserViewModel();
  role: string = JSON.parse(sessionStorage.getItem('role'));
  isLoaded:boolean=false;

  constructor( public profileService: ProfileService,
               private route: ActivatedRoute,
               public authService: AuthenticationService){}


  ngOnInit(): void {
    this.userId = this.profileService.userId.toUpperCase();

    if(this.role === 'customer')
    {
      this.getUser()
    }
    else{
      this.getLibrarian()
    }
  }

  getUser()
  {
    this.profileService.getUserById(this.userId).subscribe({
      next: (data: UserViewModel) => {
        this.user = data;
      }
    })
  } 

  getLibrarian(){
    this.profileService.getLibrarianById(this.userId).subscribe((data: UserViewModel)=> {
      this.user = data;
    })
  }
}
