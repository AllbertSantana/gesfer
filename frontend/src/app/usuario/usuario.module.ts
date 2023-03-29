import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsuarioRoutingModule } from './usuario-routing.module';
import { UsuarioListComponent } from './usuario-list/usuario-list.component';
import { UsuarioMaterialModule } from './usuario-material.module';
import { UsuarioFormComponent } from './usuario-form/usuario-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TextMaskModule } from 'angular2-text-mask';


@NgModule({
  declarations: [
    UsuarioListComponent,
    UsuarioFormComponent
  ],
  imports: [
    CommonModule,
    UsuarioRoutingModule,
    UsuarioMaterialModule,
    ReactiveFormsModule,
    TextMaskModule,
  ]
})
export class UsuarioModule { }
