import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from '../../helpers/auth.guard';
import { CreateComponent } from './create/create.component';
import { DetailsComponent } from './details/details.component';
import { IndexComponent } from './index/index.component';
import { UpdateComponent } from './update/update.component';

const routes: Routes = [
  { path: 'author', 
  redirectTo: 'author/index', 
  pathMatch: 'full',
},
  { path: 'author/index', component: IndexComponent,canActivate:[AuthGuardService] },
  { path: 'author/:authorId/details', component: DetailsComponent,canActivate:[AuthGuardService] },
  { path: 'author/create', component: CreateComponent, canActivate:[AuthGuardService] },
  { path: 'author/:authorId/update', component: UpdateComponent, canActivate:[AuthGuardService] } 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthorRoutingModule { }
