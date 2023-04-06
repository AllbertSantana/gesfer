import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioRoutingModule } from './usuario-routing.module';
import { UsuarioListComponent } from './usuario-list/usuario-list.component';
import { UsuarioFormComponent } from './usuario-form/usuario-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TextMaskModule } from 'angular2-text-mask';
import { SharedModule } from '../shared/shared.module';
import { UsuarioDetailComponent } from './usuario-detail/usuario-detail.component';


@NgModule({
  declarations: [
    UsuarioListComponent,
    UsuarioFormComponent,
    UsuarioDetailComponent,
  ],
  imports: [
    CommonModule,
    UsuarioRoutingModule,
    ReactiveFormsModule,
    TextMaskModule,
    SharedModule,
  ]
})
export class UsuarioModule { }
