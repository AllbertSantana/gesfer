import { Component, OnInit } from '@angular/core';
import { UsuarioDetailed, UsuarioFormValue } from '../model/usuario';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestService } from '../request.service';
import { finalize, take } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { FormService } from 'src/app/shared/services/form/form.service';
import { initialInputsConfigs } from '../input-configs';
import { FormType, InputConfig, InputControlConfig } from 'src/app/shared/model/form';

@Component({
  selector: 'app-usuario-form',
  templateUrl: './usuario-form.component.html',
  styleUrls: ['./usuario-form.component.css']
})
export class UsuarioFormComponent implements OnInit {
  public initialFilterConfig: InputConfig[] = initialInputsConfigs.filter(config => config.control.usedAt.some(form => form === FormType.edit));
  public isAddForm!: boolean;
  public form!: FormGroup<any>;
  public usuario?: UsuarioDetailed;
  public testForm = new FormGroup({
    testPerfil: new FormControl('Beta', {nonNullable: true, validators: [Validators.required]})
  });

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    public authService: AuthService,
    private _formService: FormService,
    public requestService: RequestService,
  ) {}

  ngOnInit(): void {
    this.checkIdParam(this._route.snapshot.paramMap.get('id'));
  }

  checkIdParam(id: string | null): void {
    if (id) {
      this.requestService.getUsuarioById(parseInt(id))
        .pipe(take(1))
        .subscribe(
          (usuario) => {
            if (usuario) {
              this.usuario = usuario;
              this.isAddForm = false;
              this.createForm(usuario);
            } else {
              this._router.navigateByUrl('/usuarios');
            }
          }
        );
    } else {
      this.isAddForm = true;
      this.createForm();
    }
  }

  createForm(usuario?: UsuarioDetailed): void {
    if(usuario) {
      let addFormControls: InputControlConfig[] = this.initialFilterConfig.map(
        config => {
          return {
            ...config.control,
            value: usuario[config.control.name] ? usuario[config.control.name].toString().toLowerCase() : ''
          }
        });

      this.form = this._formService.toFormGroup(addFormControls);
    } else {
      let addFormControls: InputControlConfig[] = this.initialFilterConfig.map(config => config.control);
      this.form = this._formService.toFormGroup(addFormControls);
    }
  }

  onSubmit(): void {
    this.form.disable();

    let usuarioModificado: UsuarioFormValue = {
      id: this.usuario ? this.usuario.id : 0,
      nome: this.form.value.nome!,
      cpf: this.form.value.cpf!,
      email: this.form.value.email!,
      perfil: this.form.value.perfil!,
      senha: this.form.value.senha!
    };

    if (this.isAddForm) {
      this.requestService.addUsuario(usuarioModificado)
        .pipe(
          take(1),
          finalize(() => this.form.enable())
        )
        .subscribe();
    } else {
      this.requestService.updateUsuario(usuarioModificado)
        .pipe(
          take(1),
          finalize(() => this.form.enable())
        )
        .subscribe(
          usuario => {
            if(usuario?.cpf === this.authService.userInfo?.cpf) {
              this.authService.logout();
              this._router.navigateByUrl('login');
            }
          }
        );
    }
  }
}
