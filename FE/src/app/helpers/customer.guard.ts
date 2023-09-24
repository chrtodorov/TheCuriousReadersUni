import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

import { AuthenticationService } from "../services/authentication.service";


@Injectable({providedIn: 'root'})
export class CustomerGuardService implements CanActivate {
    constructor(public authService: AuthenticationService, public router: Router, private toastr: ToastrService) { }
    
    canActivate(): boolean {
        if (this.authService.getRole() !== 'customer') {
            this.toastr.error('You do not have permissions to visit this route');
            this.router.navigate(['/']);
            return false;
        }
        return true;
    }
}