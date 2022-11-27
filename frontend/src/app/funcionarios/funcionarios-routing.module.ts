import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FuncionarioAddComponent } from './funcionario-add/controller/funcionario-add.component';
import { FuncionarioEditComponent } from './funcionario-edit/controller/funcionario-edit.component';
import { FuncionariosListComponent } from './funcionarios-list/funcionarios-list.component';

const routes: Routes = [
  {
    path: 'funcionarios',
    component: FuncionariosListComponent,
    children: [
      { path: 'editar/:id', component: FuncionarioEditComponent },
      { path: 'adicionar', component: FuncionarioAddComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FuncionariosRoutingModule { }