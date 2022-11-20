import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public funcionarios?: Funcionario[];

  constructor(http: HttpClient) {
    http.get<Funcionario[]>('/api/funcionario').subscribe(result => {
      this.funcionarios = result;
    }, error => console.error(error));
  }

  title = 'frontend';
}

interface Funcionario {
  id: number;
  nome: string;
  cpf: string;
  matricula: string;
  dataVinculo: string;
}
