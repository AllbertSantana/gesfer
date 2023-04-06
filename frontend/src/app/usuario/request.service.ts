import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MessageService } from "../shared/services/message/message.service";
import { BehaviorSubject, Observable, catchError, finalize, map, of, tap } from "rxjs";
import { FiltersValues, Usuario, UsuarioBackendErrors, UsuarioDetailed, UsuarioFormValue, UsuarioRequestParams, UsuarioResponse, UsuarioUnprocessableEntityResponse } from "./model/usuario";
import { Message, MessageTypes } from "../shared/model/message";
import { AuthService } from "../shared/services/auth/auth.service";

@Injectable({
    providedIn: 'root',
})
export class RequestService {
    private _loading$ = new BehaviorSubject<boolean>(false);
    public loading$ = this._loading$.asObservable();
    private _allPagesSize$ = new BehaviorSubject<number>(0);
    public allPagesSize$ = this._allPagesSize$.asObservable();

    constructor(
        private http: HttpClient,
        private authService: AuthService,
        private messenger: MessageService,
    ) {}

    getUsuarios(
        pageNumber = 1,
        pageSize = 5,
        filters: FiltersValues = {nome: '', cpf: '', email: '', perfil: ''},
        orderBy = 'nome',
        order = 'asc',
    ): Observable<Usuario[]> {
        this._loading$.next(true);

        let paramsObj: {[k: string]: any} = {
            PageNumber: pageNumber.toString(),
            PageSize: pageSize.toString(),
            OrderBy: orderBy,
            Order: order
        };
        
        for (const key in filters) {
            if (filters[key as keyof FiltersValues]) {
                paramsObj[`Filter.${key}` as keyof UsuarioRequestParams] = filters[key as keyof FiltersValues];
            }
        }

        return this.http.get<UsuarioResponse>('/api/usuario', {params: new HttpParams({fromObject: paramsObj})})
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {
                        this.messenger.notify({
                            title: 'Houve um erro ao buscar funcionários',
                            type: MessageTypes.error
                        });
                        return of(null);
                    }
                ),
                map(
                    res => {
                        if (res) {
                            this._allPagesSize$.next(res.rowCount);
                            return res.items;
                        } else {
                            return [];
                        }
                    }
                ),
                finalize(() => this._loading$.next(false))
            )
    }

    getUsuarioById(id: number): Observable<UsuarioDetailed | null> {
        this._loading$.next(true);

        return this.http.get<UsuarioDetailed>(`/api/usuario/${id}`)
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {
                        this.messenger.notify({
                            title: 'Houve um erro ao buscar usuário',
                            type: MessageTypes.error
                        });
                        return of(null);
                    }
                ),
                finalize(() => this._loading$.next(false))
            );
    }

    addUsuario(usuario: UsuarioFormValue): Observable<UsuarioDetailed | null> {
        this._loading$.next(true);
    
        return this.http.post<UsuarioDetailed>('/api/usuario', usuario)
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {
                        let errorBody: UsuarioUnprocessableEntityResponse = error.error;
                        let errorDesc: string[] = [];
                
                        if (error.status < 500) {
                            for (const prop in errorBody.errors) {
                            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof UsuarioBackendErrors]?.[0]}`);
                            }
                        }
                
                        let msg = {
                            title: `Houve um erro ao adicionar usuário`,
                            type: MessageTypes.error,
                            errors: (error.status < 500) ? errorDesc : [''],
                        };
                
                        this.messenger.notify(msg);
                        
                        return of(null);
                    }
                ),
                tap(
                    (res) => {
                        if (res) {
                            let msg = {
                            title: `Usuário Adicionado`,
                            type: MessageTypes.success,
                            };
                
                            this.messenger.notify(msg);
                        }
                    }
                ),
                finalize(() => this._loading$.next(false))
            );
    }

    updateUsuario(usuario: UsuarioFormValue): Observable<UsuarioDetailed | null> {
        this._loading$.next(true);
    
        return this.http.patch<UsuarioDetailed>(`/api/usuario/${usuario.id}`, usuario)
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {
                        let errorBody: UsuarioUnprocessableEntityResponse = error.error;
                        let errorDesc: string[] = [];
                
                        if (error.status < 500) {
                            for (const prop in errorBody.errors) {
                            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof UsuarioBackendErrors]?.[0]}`);
                            }
                        }
                
                        let msg = {
                            title: `Houve um erro ao atualizar usuário`,
                            type: MessageTypes.error,
                            errors: (error.status < 500) ? errorDesc : [''],
                        };
                
                        this.messenger.notify(msg);
                        
                        return of(null);
                    }
                ),
                tap(
                    (res) => {
                        if (res) {
                            var msg!: Message;
                            
                            if (this.authService.userInfo?.cpf === usuario.cpf) {
                                msg = {
                                    title: 'Seu usuário foi Atualizado',
                                    type: MessageTypes.success,
                                    subtitle: 'Faça Login novamente'
                                };
                            } else {
                                msg = {
                                    title: 'Usuário Atualizado',
                                    type: MessageTypes.success,
                                };
                            }
                
                            this.messenger.notify(msg);
                        }
                    }
                ),
                finalize(() => this._loading$.next(false))
            );
    }

    removeUsuario(usuario: Usuario): Observable<UsuarioDetailed | null> {
        this._loading$.next(true);
    
        return this.http.delete<UsuarioDetailed>(`/api/usuario/${usuario.id}`)
            .pipe(
                catchError(
                    (error: HttpErrorResponse) => {
                        let errorBody: UsuarioUnprocessableEntityResponse = error.error;
                        let errorDesc: string[] = [];
                
                        if (error.status < 500) {
                            for (const prop in errorBody.errors) {
                                errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof UsuarioBackendErrors]?.[0]}`);
                            }
                        }
                
                        let msg = {
                            title: `Houve um erro ao remover usuário`,
                            type: MessageTypes.error,
                            errors: (error.status < 500) ? errorDesc : [''],
                        };
                
                        this.messenger.notify(msg);
                        
                        return of(null);
                    }
                ),
                tap(
                    (res) => {
                        if (res) {
                            var msg!: Message;
                            
                            if (this.authService.userInfo?.cpf === usuario.cpf) {
                                msg = {
                                    title: 'Seu usuário foi removido',
                                    type: MessageTypes.success,
                                    subtitle: 'Faça Login novamente'
                                };
                            } else {
                                msg = {
                                    title: 'Usuário Removido',
                                    type: MessageTypes.success,
                                };
                            }
                
                            this.messenger.notify(msg);
                        }
                    }
                ),
                finalize(() => this._loading$.next(false))
            );
    }
}