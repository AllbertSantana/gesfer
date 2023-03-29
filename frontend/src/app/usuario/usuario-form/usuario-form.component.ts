import { Component, OnInit } from '@angular/core';
import { UsuarioDetailed, UsuarioFormValue } from '../model/usuario';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UsuarioService } from '../usuario.service';
import { finalize, take } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-usuario-form',
  templateUrl: './usuario-form.component.html',
  styleUrls: ['./usuario-form.component.css']
})
export class UsuarioFormComponent implements OnInit {
  public cpfMask = [/\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '-', /\d/, /\d/];
  public isAddForm: boolean = true;
  public form = new FormGroup({
    nome: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
    cpf: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(14), Validators.maxLength(14)]}),
    email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(11), Validators.maxLength(100)]}),
    perfil: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
    senha: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(8), Validators.maxLength(50)]})
  });
  private _usuario?: UsuarioDetailed;

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _authService: AuthService,
    public usuarioService: UsuarioService,
  ) {}

  ngOnInit(): void {
    this.checkIdParam(this._route.snapshot.paramMap.get('id'));
  }

  checkIdParam(id: string | null): void {
    if (id) {
      this.usuarioService.getUsuarioById(parseInt(id))
        .pipe(take(1))
        .subscribe(
          (usuario) => {
            if (usuario) {
              this._usuario = usuario;
              this.isAddForm = false;
              this.fillForm(usuario);
            } else {
              this._router.navigateByUrl('/usuarios');
            }
          }
        );
    }
  }

  fillForm(usuario: UsuarioDetailed): void {
    this.form.patchValue({
      nome: usuario.nome,
      cpf: usuario.cpf,
      email: usuario.email,
      perfil: usuario.perfil
    });
  }

  onSubmit(): void {
    this.form.disable();

    let usuarioModificado: UsuarioFormValue = {
      id: this._usuario ? this._usuario.id : 0,
      nome: this.form.value.nome!,
      cpf: this.form.value.cpf!,
      email: this.form.value.email!,
      perfil: this.form.value.perfil!,
      senha: this.form.value.senha!
    };

    if (this.isAddForm) {
      this.usuarioService.addUsuario(usuarioModificado)
        .pipe(
          take(1),
          finalize(() => this.form.enable())
        )
        .subscribe();
    } else {
      this.usuarioService.updateUsuario(usuarioModificado)
      .pipe(
        take(1),
        finalize(() => this.form.enable())
      )
      .subscribe(
        usuario => {
          if(usuario?.cpf === this._authService.userInfo?.cpf) {
            this._authService.logout();
            this._router.navigateByUrl('login');
          }
        }
      );
    }
  }
}
