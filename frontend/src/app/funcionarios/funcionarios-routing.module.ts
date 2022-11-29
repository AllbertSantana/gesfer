import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FuncionarioAddComponent } from './funcionario-add/funcionario-add.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { FuncionariosListComponent } from './funcionarios-list/funcionarios-list.component';

const routes: Routes = [
  {
    path: 'funcionarios',
    children: [
      { path: '', component: FuncionariosListComponent },
      { path: 'adicionar', component: FuncionarioAddComponent },
      { path: 'editar/:id', component: FuncionarioEditComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FuncionariosRoutingModule { }