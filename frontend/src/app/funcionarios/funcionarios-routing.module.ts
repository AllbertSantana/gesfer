import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FuncionarioAddComponent } from './funcionario-add/funcionario-add.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { FuncionarioListComponent } from './funcionario-list/funcionario-list.component';
import { ExercicioListComponent } from './exercicio-list/exercicio-list.component';
import { ExercicioAddComponent } from './exercicio-add/exercicio-add.component';
import { ExercicioEditComponent } from './exercicio-edit/exercicio-edit.component';

const routes: Routes = [
  {
    path: 'funcionarios',
    children: [
      { path: '', component: FuncionarioListComponent },
      { path: 'adicionar', component: FuncionarioAddComponent },
      { path: ':id/editar', component: FuncionarioEditComponent },
      { path: ':id/exercicios', component: ExercicioListComponent },
      { path: ':id/exercicios/adicionar', component: ExercicioAddComponent },
      { path: ':funcionarioId/exercicios/:exercicioId/editar', component: ExercicioEditComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FuncionariosRoutingModule { }