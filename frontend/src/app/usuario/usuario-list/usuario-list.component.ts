import { Component, OnDestroy, OnInit } from '@angular/core';
import { UsuarioService } from '../usuario.service';
import { Usuario } from '../model/usuario';
import { BehaviorSubject, take } from 'rxjs';
import { UsuarioDataSource } from '../implementations/usuario-data-source';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { RemoveDialogComponent } from 'src/app/shared/components/remove/remove-dialog.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css']
})
export class UsuarioListComponent implements OnInit, OnDestroy {
  public dataSource: UsuarioDataSource;
  public displayedColumns = ['cpf', 'nome', 'email'];
  public pageSize = 5;
  public pageSizeOptions = [5, 10, 15, 20];
  public selectedUsuarios: Map<number, Usuario> = new Map<number, Usuario>();

  constructor(
    private _router: Router,
    public usuarioService: UsuarioService,
    public authService: AuthService,
    public dialog: MatDialog,
  ) {
    this.dataSource = new UsuarioDataSource(this.usuarioService);
  }

  ngOnInit(): void {
    this.dataSource.loadUsuarios();
  }

  ngOnDestroy(): void {
    this.selectedUsuarios.clear();
  }

  onUsuarioSelect(usuario: Usuario): void {
    if (this.selectedUsuarios.has(usuario.id)) {
      this.selectedUsuarios.delete(usuario.id);
    } else {
      this.selectedUsuarios.clear();
      this.selectedUsuarios.set(usuario.id, usuario);
    }
  }

  getSelectedUsuario(): Usuario {
    let usuarios: Usuario[] = [];
    for (let row of this.selectedUsuarios.values()) { usuarios.push(row); }
    return usuarios[0];
  }

  openRemoveDialog(): void {
    let usuario: Usuario = this.getSelectedUsuario();
    const removeDialogRef = this.dialog.open(RemoveDialogComponent, {data: {usuario: usuario}});
    
    removeDialogRef.afterClosed()
      .pipe(take(1))
      .subscribe(
        (proceeded: boolean) => {
          if (proceeded) {
            this.usuarioService.removeUsuario(usuario)
              .pipe(take(1))
              .subscribe(
                (_) => {
                  if (this.getSelectedUsuario().cpf === this.authService.userInfo?.cpf) {
                    this.authService.logout();
                    this._router.navigateByUrl('login');
                  } else {
                    this.selectedUsuarios.clear();
                    this.dataSource.loadUsuarios();
                  }
                }
              )
          }
        }
      );
  }
}