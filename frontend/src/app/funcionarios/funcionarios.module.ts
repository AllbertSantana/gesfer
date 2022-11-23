import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FuncionariosRoutingModule } from './funcionarios-routing.module';
import { FuncionariosListComponent } from './funcionarios-list/funcionarios-list.component';
import { FuncionariosMaterialModule } from './funcionarios-material.module';


@NgModule({
  declarations: [
    FuncionariosListComponent
  ],
  imports: [
    CommonModule,
    FuncionariosRoutingModule,
    FuncionariosMaterialModule
  ]
})
export class FuncionariosModule { }
