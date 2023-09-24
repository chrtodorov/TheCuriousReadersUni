import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountComponent } from './components/account-approval-page/account.component';
import { ProfileDetailsComponent } from './components/authentication/profile-details/profile-details.component';
import { BookListComponent } from './components/book-list/book-list.component';
import { LoginComponent } from './components/authentication/login/login.component';
import { RegisterComponent } from './components/authentication/register/register.component';

import { AuthGuardService } from './helpers/auth.guard';
import { HomepageComponent } from './homepage/homepage.component';
import { PendingCommentsComponent } from './components/pending-comments/pending-comments.component';

const routes: Routes = [
  { path: '', component:HomepageComponent, pathMatch: 'full' },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'account-approve',
    component: AccountComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  { 
    path: 'books', 
    component: BookListComponent 
  },
  {
    path: 'profile',
    component: ProfileDetailsComponent
  },
  {
    path: 'comments/pending',
    component: PendingCommentsComponent,
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
