import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from '../../helpers/auth.guard';
import { CreateComponent } from './create/create.component';
import { DetailsComponent } from './details/details.component';
import { IndexComponent } from './index/index.component';
import { UpdateComponent } from './update/update.component';

const routes: Routes = [
  { path: 'publisher', redirectTo: 'publisher/index', pathMatch: 'full'},
  { path: 'publisher/index', component: IndexComponent,canActivate:[AuthGuardService] },
  { path: 'publisher/:publisherId/details', component: DetailsComponent,canActivate:[AuthGuardService] },
  { path: 'publisher/create', component: CreateComponent, canActivate:[AuthGuardService]},
  { path: 'publisher/:publisherId/update', component: UpdateComponent, canActivate:[AuthGuardService]} 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublisherRoutingModule { }
