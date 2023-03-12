import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../shared/services/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, Observable, Subject, finalize, map, pipe, take, takeUntil } from 'rxjs';
import { HttpResponse } from '@angular/common/http';
import { MessageService } from '../shared/services/message/message.service';
import { MessageTypes } from '../shared/model/message';
import jwt_decode from "jwt-decode";
import { JwtPayload, Login } from '../shared/model/usuario';
import { emailMask } from 'text-mask-addons';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;

  loginForm: FormGroup;
  useEmailAsUsername = new BehaviorSubject<boolean>(true);
  
  public cpfMask = [/\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '-', /\d/, /\d/];
  public emailMask = emailMask;

  hidePassword = true;
  redirectURL!: Observable<string>;

  constructor(
    public authService: AuthService,
    private messenger: MessageService,
    public router: Router,
  ) {
    this.loginForm = new FormGroup({
      cpf: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(14), Validators.maxLength(14)]}),
      email: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(11), Validators.maxLength(100)]}),
      senha: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(8), Validators.maxLength(18)]})
    });
  }

  ngOnInit(): void {
    this.useEmailAsUsername
      .pipe(takeUntil(this.destroyed$))  
      .subscribe(
        useEmail => {
          console.log('     ', 'dirty', 'touched');
          console.log('cpf  ', this.cpf.dirty, this.cpf.touched);
          console.log('email', this.email.dirty, this.email.touched);
          console.log('senha', this.senha.dirty, this.senha.touched);

          if (useEmail) {
            this.cpf.reset();
            this.cpf.disable();
            this.email.enable();
          } else {
            this.email.reset();
            this.email.disable();
            this.cpf.enable();
          }
        }
      );
  }

  ngOnDestroy(): void {
    this.authService.redirectUrl = undefined;
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get cpf() { return this.loginForm.get('cpf')!; }
  get email() { return this.loginForm.get('email')!; }
  get senha() { return this.loginForm.get('senha')!; }

  login(login: Login) {
    this.authService.login(login)
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => {
          this.senha.enable();

          if (this.useEmailAsUsername.getValue()) {
            this.email.enable();
          } else {
            this.cpf.enable();
          }
        })
      )
      .subscribe((res: HttpResponse<any>) => {
        if (res && res.ok) {
          try {
            let redirectUrl = this.authService.redirectUrl;
            let authToken = res.headers.get('authorization')!;
            let decodedAuthToken = jwt_decode<JwtPayload>(authToken);

            this.authService.isLoggedIn = true;
            this.authService.userInfo = res.body;
            this.authService.setToken(authToken);
            this.authService.decodedToken = decodedAuthToken;

            if (redirectUrl) {
              this.router.navigate([redirectUrl]);
            } else {
              this.router.navigate(['/']);
            }
          } catch (error) {
            this.messenger.notify({
              title: 'Token de autenticação inválido.',
              type: MessageTypes.error
            });
          }
        } else {
          this.messenger.notify({
            title: 'Não foi possível fazer login.',
            type: MessageTypes.error
          });
        }
    });
  }

  switchUsernameMethod() {
    let useEmail = this.useEmailAsUsername.getValue();
    this.useEmailAsUsername.next(!useEmail);
  }

  onSubmit() {
    this.loginForm.disable();

    let login: Login = {
      cpf: this.loginForm.value.cpf,
      email: this.loginForm.value.email,
      senha: this.loginForm.value.senha
    };

    this.login(login);
  }
}
