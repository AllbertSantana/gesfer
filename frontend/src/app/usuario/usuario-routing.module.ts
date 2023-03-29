import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsuarioListComponent } from './usuario-list/usuario-list.component';
import { adminAuthorizationGuard, authenticationGuard, modifyAuthorizationGuard } from '../shared/guards/auth.guard';
import { UsuarioFormComponent } from './usuario-form/usuario-form.component';

const routes: Routes = [
  {
    path: 'usuarios',
    canActivateChild: [authenticationGuard, modifyAuthorizationGuard, adminAuthorizationGuard],
    children: [
      { path: '', component: UsuarioListComponent },
      { path: 'adicionar', component: UsuarioFormComponent },
      { path: ':id/editar', component: UsuarioFormComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsuarioRoutingModule { }
