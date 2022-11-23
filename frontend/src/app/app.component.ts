import { HttpClient, HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor() {}
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
