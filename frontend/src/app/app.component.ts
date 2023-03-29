import { HttpClient, HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public currentYear = new Date().getFullYear();

  constructor(
    public authService: AuthService,
    public router: Router
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isAdmin(): boolean {
    const adminPermissions = [
      "BuscarUsuario",
      "ListarUsuarios",
      "RemoverUsuario",
      "CriarUsuario",
      "EditarUsuario"
    ];

    if (
      adminPermissions.every(p => this.authService.userInfo?.permissoes.includes(p))
    ) { return true; } else return false;
  }
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
