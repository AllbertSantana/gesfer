import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FuncionariosRoutingModule } from './funcionarios-routing.module';
import { FuncionariosListComponent } from './funcionarios-list/funcionarios-list.component';
import { FuncionariosMaterialModule } from './funcionarios-material.module';
import { FuncionarioEditComponent } from './funcionario-edit/controller/funcionario-edit.component';
import { FuncionarioEditDialogComponent } from './funcionario-edit/dialog/funcionario-edit-dialog.component';
import { FuncionarioAddComponent } from './funcionario-add/controller/funcionario-add.component';
import { FuncionarioAddDialogComponent } from './funcionario-add/dialog/funcionario-add-dialog.component';


@NgModule({
  declarations: [
    FuncionariosListComponent,
    FuncionarioEditComponent,
    FuncionarioEditDialogComponent,
    FuncionarioAddComponent,
    FuncionarioAddDialogComponent,
  ],
  imports: [
    CommonModule,
    FuncionariosRoutingModule,
    FuncionariosMaterialModule
  ]
})
export class FuncionariosModule { }
