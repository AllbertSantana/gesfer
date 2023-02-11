import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { FuncionariosRoutingModule } from './funcionarios-routing.module';
import { FuncionarioListComponent } from './funcionario-list/funcionario-list.component';
import { FuncionariosMaterialModule } from './funcionarios-material.module';
import { FuncionarioAddComponent } from './funcionario-add/funcionario-add.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { FuncionarioFilterDialogComponent } from './funcionario-filter/funcionario-filter-dialog.component';
import { IMaskModule } from 'angular-imask';
import { ExercicioListComponent } from './exercicio-list/exercicio-list.component';
import { ExercicioAddComponent } from './exercicio-add/exercicio-add.component';
import { ExercicioEditComponent } from './exercicio-edit/exercicio-edit.component';
import { RemoveDialogComponent } from './remove/remove-dialog.component';

@NgModule({
  declarations: [
    FuncionarioListComponent,
    FuncionarioEditComponent,
    FuncionarioAddComponent,
    FuncionarioFilterDialogComponent,
    RemoveDialogComponent,
    ExercicioListComponent,
    ExercicioAddComponent,
    ExercicioEditComponent,
  ],
  imports: [
    CommonModule,
    FuncionariosRoutingModule,
    ReactiveFormsModule,
    FuncionariosMaterialModule,
    IMaskModule,
  ]
})
export class FuncionarioModule { }
