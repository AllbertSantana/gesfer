import { Component, OnInit } from '@angular/core';
import { RequestService } from '../request.service';
import { UsuarioDetailed } from '../model/usuario';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'src/app/shared/services/message/message.service';
import { MessageTypes } from 'src/app/shared/model/message';
import { take } from 'rxjs';

@Component({
  selector: 'app-usuario-detail',
  templateUrl: './usuario-detail.component.html',
  styleUrls: ['./usuario-detail.component.css']
})
export class UsuarioDetailComponent implements OnInit {
  public usuario?: UsuarioDetailed;

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private message: MessageService,
    public requestService: RequestService,
  ) {}

  ngOnInit(): void {
    const id = this._route.snapshot.paramMap.get('id');
    let isParamOk = this.checkRouteParam(id);

    if (isParamOk) {
      this.getUsuario(parseInt(id!, 10))
    } else {
      this._router.navigateByUrl('/usuarios');
      this.message.notify({ title: 'ID requisitado é inválido', type: MessageTypes.error });
    }
  }

  checkRouteParam(id: string | null): boolean {
    if (id === null || isNaN(parseInt(id, 10))) {
      return false;
    } else {
      return true;
    }
  }

  getUsuario(id: number) {
    this.requestService.getUsuarioById(id)
      .pipe(take(1))
      .subscribe(
        usuario => {
          if(usuario) {
            this.usuario = usuario;
          } else {
            this._router.navigateByUrl('/usuarios');
            this.message.notify({ title: 'Usuário não encontrado', type: MessageTypes.error });
          }
        }
      );
  }
}
