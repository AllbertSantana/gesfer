import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, retry, tap, throwError } from 'rxjs';
import { MessageTypes } from '../shared/model/message';
import { MessageService } from '../shared/services/message/message.service';
import { Filters, FiltersValues, Funcionario, FuncionarioUnprocessableEntityResponse, FuncionarioRequestParams, FuncionarioResponse, FuncionarioBackendErrors } from './model/funcionario';

@Injectable({
  providedIn: 'root'
})
export class FuncionariosService {  
  private _length$ = new BehaviorSubject<number>(0);
  private _filters$ = new BehaviorSubject<Filters>({
    nome: {value: '', disabled: true},
    cpf: {value: '', disabled: true},
    matricula: {value: '', disabled: true},
  });
  private _hasFilter$ = new BehaviorSubject<boolean>(false);

  constructor(
    private http: HttpClient,
    private messenger: MessageService,
  ) { }

  getFuncionarios(
    pageNumber = 1,
    pageSize = 5,
    filters: FiltersValues = {nome: '', cpf: '', matricula: ''},
    orderBy = 'nome',
    order = 'asc',
  ): Observable<Funcionario[]> {
    let paramsObj: {[k: string]: any} = {
      PageNumber: pageNumber.toString(),
      PageSize: pageSize.toString(),
      OrderBy: orderBy,
      Order: order
    };
    
    for (const key in filters) {
      if (filters[key as keyof FiltersValues]) {
        paramsObj[`Filter.${key}` as keyof FuncionarioRequestParams] = filters[key as keyof FiltersValues];
      }
    }

    return this.http.get<FuncionarioResponse>(
      '/api/funcionario',
      {
        params: new HttpParams({fromObject: paramsObj})
      }
    )
    .pipe(
      catchError((error: HttpErrorResponse) => {
        this.messenger.notify({
          title: 'Houve um erro ao buscar funcionários',
          type: MessageTypes.error
        });
        return of(null);
      }),
      map(res => {
        if (res) {
          this._length$.next(res.rowCount);
          return res.items;
        } else {
          return [];
        }
      })
    )
  }

  getFuncionarioById(id: string): Observable<Funcionario | null> {
    return this.http.get<Funcionario | null>(`/api/funcionario/${id}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.messenger.notify({
            title: 'Houve um erro ao buscar funcionário',
            type: MessageTypes.error
          });
          return of(null);
        })
      );
  }

  addFuncionario(funcionario: Funcionario): Observable<Funcionario | null> {
    return this.http.post<Funcionario>(
      '/api/funcionario',
      funcionario,
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorBody: FuncionarioUnprocessableEntityResponse = error.error;
        let errorDesc: string[] = [];

        if (error.status < 500) {
          for (const prop in errorBody.errors) {
            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof FuncionarioBackendErrors]?.[0]}`);
          }
        }

        let msg = {
          title: `Houve um erro ao adicionar funcionário.`,
          type: MessageTypes.error,
          errors: (error.status < 500) ? errorDesc : [''],
        };

        this.messenger.notify(msg);
        
        return of(null);
      }),
      tap((res) => {
        if (res) {
          let msg = {
            title: `Funcionário Adicionado.`,
            type: MessageTypes.success,
          };

          this.messenger.notify(msg);
        }
      })
    );
  }

  updateFuncionario(funcionario: Funcionario) {
    return this.http.put<Funcionario>(
      `/api/funcionario/${funcionario.id}`,
      funcionario
    )
    .pipe(
      catchError((error: HttpErrorResponse) => {
        let errorBody: FuncionarioUnprocessableEntityResponse = error.error;
        let errorDesc: string[] = [];

        if (error.status < 500) {
          for (const prop in errorBody.errors) {
            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof FuncionarioBackendErrors]?.[0]}`);
          }
        }

        let msg = {
          title: `Houve um erro ao atualizar funcionário.`,
          type: MessageTypes.error,
          errors: (error.status < 500) ? errorDesc : [''],
        };

        this.messenger.notify(msg);
        
        return of(null);
      }),
      tap((res) => {
        if (res) {
          let msg = {
            title: `Funcionário Atualizado.`,
            type: MessageTypes.success,
          };

          this.messenger.notify(msg);
        }
      })
    );
  }

  removeFuncionario(funcionario: Funcionario) {
    return this.http.delete<Funcionario[]>(
      `/api/funcionario/${funcionario.id}`
    )
      .pipe(
        catchError((error: HttpErrorResponse) => {
          let errorBody: FuncionarioUnprocessableEntityResponse = error.error;
          let errorDesc: string[] = [];
  
          if (error.status < 500) {
            for (const prop in errorBody.errors) {
              errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof FuncionarioBackendErrors]?.[0]}`);
            }
          }
  
          let msg = {
            title: `Houve um erro ao remover funcionário.`,
            type: MessageTypes.error,
            errors: (error.status < 500) ? errorDesc : [''],
          };
  
          this.messenger.notify(msg);
          
          return of(null);
        }),
        tap((res) => {
          if (res) {
            let msg = {
              title: `Funcionário Removido.`,
              type: MessageTypes.success,
            };
  
            this.messenger.notify(msg);
          }
        })
      );
  }

  getRowCount(): Observable<number> {
    return this._length$.asObservable();
  }

  getFilters(): Observable<Filters>{
    return this._filters$.asObservable();
  }

  updateFilters(newFilters: Filters): void {
    this._filters$.next(newFilters);
  }

  clearFilters(): void {
    let emptyFilters = {
      nome: {value: '', disabled: true},
      cpf: {value: '', disabled: true},
      matricula: {value: '', disabled: true},
    };

    this._filters$.next(emptyFilters);
  }

  getHasFilter(): Observable<boolean> {
    return this._hasFilter$.asObservable();
  }

  updateHasFilter(boolean: boolean): void {
    this._hasFilter$.next(boolean);
  }
}