import { Injectable } from '@angular/core';
import { BehaviorSubject, EMPTY, Observable, iif, interval, of } from 'rxjs';
import { tap, catchError, switchMap, take, finalize } from 'rxjs/operators';
import { JwtPayload, Login, Usuario } from '../../model/usuario';
import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { MessageService } from '../message/message.service';
import { MessageTypes } from '../../model/message';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _loading$ = new BehaviorSubject<boolean>(false);
  public loading$ = this._loading$.asObservable();

  public isLoggedIn = false;
  public userInfo: Usuario | undefined;
  public redirectUrl: string | undefined;
  public decodedToken: JwtPayload | undefined;
  private token = new BehaviorSubject<string | null>(null);
  private tokenRenewalInterval$ = this.token
    .asObservable()
    .pipe(
      switchMap(
        () => iif(
          () => this.isLoggedIn,
          interval(1140000), // 1140000 = 19 minutes; thats 1 min before token expires
          EMPTY
        )
      )
  );

  constructor(
    private http: HttpClient,
    private messenger: MessageService,
  ) {
    this.setTokenRenewalTimer();
  }

  login(login: Login): Observable<any | null> {
    this._loading$.next(true);

    return this.http.post<Usuario>(
      '/api/usuario/token',
      login,
      { observe: 'response' as 'body'}
    ).pipe(
      catchError((errorRes: HttpErrorResponse) => { return of(null); }),
      finalize(() => this._loading$.next(false))
    );
  }

  logout(): void {
    this.isLoggedIn = false;
    this.userInfo = undefined;
    this.unsetToken();
  }

  renewToken(): Observable<any | null> {
    this._loading$.next(true);

    return this.http.head(
      '/api/usuario/token',
      { observe: 'response' as 'body'}
    ).pipe(
      catchError((errorRes: HttpErrorResponse) => {
        this.logout();
        this.messenger.notify({
          title: 'Houve um erro ao renovar sua sessão de usuário.',
          subtitle: 'Faça login novamente.',
          type: MessageTypes.warning
        });

        return of(null);
      }),
      finalize(() => this._loading$.next(false))
    );
  }

  setToken(token: string): void {
    this.token.next(token);
  }

  unsetToken(): void {
    this.token.next(null);
  }

  getToken(): String | null {
    return this.token.value;
  }

  setTokenRenewalTimer() {
    return this.tokenRenewalInterval$.subscribe(
      () => {
        this.renewToken()
          .pipe(take(1))  
          .subscribe(
            (res: HttpResponse<any>) => {
              if (res) {
                let jwtToken = res.headers.get('authorization');
                this.setToken(jwtToken!);
              }
            }
        );
      }
    );
  }
}