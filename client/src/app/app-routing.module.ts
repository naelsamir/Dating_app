import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './guards/auth.guard';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { preventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';
import { memberDetailedResolver } from './_resolvers/member-details.resolver';

const routes: Routes = [
  {path:'',component:HomeComponent},
  {path:'members',component:MemberListComponent, canActivate:[authGuard]},
  {path:'member/:username',component:MemberDetailsComponent, canActivate:[authGuard],resolve: {member: memberDetailedResolver}},
  {path:'member-edit',component:MemberEditComponent, canActivate:[authGuard], canDeactivate:[preventUnsavedChangesGuard]},
  {path:'lists',component:ListsComponent, canActivate:[authGuard]},
  {path:'messages',component:MessagesComponent,canActivate:[authGuard]},
  {path:'not-found',component:NotFoundComponent},
  {path:'server-error',component:ServerErrorComponent},

  {path:'**',component:NotFoundComponent,pathMatch:"full"},

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
