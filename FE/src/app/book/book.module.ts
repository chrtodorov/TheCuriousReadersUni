import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BookRoutingModule } from './book-routing.module';
import { UpdateComponent } from './update/update.component';
import { CreateComponent } from './create/create.component';
import { BookdetailsComponent } from './bookdetails/bookdetails.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { AuthenticationService } from '../services/authentication.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helpers/jwt.interceptor';
import { ToastrService } from 'ngx-toastr';
import { AuthGuardService } from '../helpers/auth.guard';
import {AutocompleteLibModule} from 'angular-ng-autocomplete';
import { BlobComponent } from './blob/blob.component';


@NgModule({
  declarations: [
    UpdateComponent,
    CreateComponent,
    BookdetailsComponent,
    BlobComponent
  ],
  imports: [
    CommonModule,
    BookRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgMultiSelectDropDownModule.forRoot(),
    NgxPaginationModule,
    AutocompleteLibModule
  ],
  providers: [AuthenticationService, ToastrService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    [AuthGuardService]
   ],
})
export class BookModule { }
