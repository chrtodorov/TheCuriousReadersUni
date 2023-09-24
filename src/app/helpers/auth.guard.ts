import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../services/authentication.service';

@Injectable()


export class AuthGuardService implements CanActivate {
  constructor(public authService:AuthenticationService, public router: Router , private toastr:ToastrService) {}
  canActivate(): boolean {
    if (this.authService.getRole()==='customer') {
      this.toastr.error('You shall not pass!');
      return false;
    }
    else if(this.authService.getRole()===null){
      this.toastr.error('Please login first!');
      return false;
    }
    return true;
  }
}