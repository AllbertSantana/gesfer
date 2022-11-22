import { HttpClient, HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public funcionarios?: Funcionario[];

  arguments: FiltroFuncionario = {
    nome: '',
    cpf: '',
    matricula: ''
  };

  constructor(http: HttpClient) {
    http.get<PlanilhaFuncionario>('/api/funcionario', {
      params: new HttpParams()
        .set('pageNumber', 1)
        .set('pageSize', 5)
        .set('orderBy', 'id')
        .set('order', 'asc')
    })
      .subscribe(result => {
        this.funcionarios = result.results;
      }, error => console.error(error));
  }

  title = 'frontend';
}

interface PlanilhaFuncionario {
  results: Funcionario[];
}

interface FiltroFuncionario {
  nome: string;
  cpf: string;
  matricula: string;
}

interface ConsultaFuncionario {
  pageNumber: number;
  pageSize: number;
  arguments: FiltroFuncionario;
  orderBy: string;
  order: string;
}

interface Funcionario {
  id: number;
  nome: string;
  cpf: string;
  matricula: string;
  dataVinculo: string;
}
