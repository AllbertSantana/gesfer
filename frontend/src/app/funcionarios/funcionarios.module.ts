import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { FuncionariosRoutingModule } from './funcionarios-routing.module';
import { FuncionariosListComponent } from './funcionarios-list/funcionarios-list.component';
import { FuncionariosMaterialModule } from './funcionarios-material.module';
import { FuncionarioAddComponent } from './funcionario-add/funcionario-add.component';
import { FuncionarioRemoveComponent } from './funcionario-remove/controller/funcionario-remove.component';
import { FuncionarioRemoveDialogComponent } from './funcionario-remove/dialog/funcionario-remove-dialog.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { CpfInputComponent } from '../shared/implementations/inputs/cpf/cpf-input.component';
import { MatriculaInputComponent } from '../shared/implementations/inputs/matricula/matricula-input.component';


@NgModule({
  declarations: [
    FuncionariosListComponent,
    FuncionarioEditComponent,
    FuncionarioAddComponent,
    FuncionarioRemoveComponent,
    FuncionarioRemoveDialogComponent,
    CpfInputComponent,
    MatriculaInputComponent,
  ],
  imports: [
    CommonModule,
    FuncionariosRoutingModule,
    ReactiveFormsModule,
    FuncionariosMaterialModule
  ],
})
export class FuncionariosModule { }
