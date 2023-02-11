import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, of, finalize, tap } from 'rxjs';

import { MessageTypes } from '../shared/model/message';
import { MessageService } from '../shared/services/message/message.service';
import { Exercicio, FuncionarioExercicios, NewExercicio, UpdatedExercicio } from './model/exercicio';
import { FuncionarioNotFoundResponse, ExercicioUnprocessableEntityResponse, ExercicioBackendErrors } from './model/funcionario';

@Injectable({
  providedIn: 'root'
})
export class FuncionarioExercicioService {
  private _loading$ = new BehaviorSubject<boolean>(false);
  public loading$ = this._loading$.asObservable();

  constructor(
    private http: HttpClient,
    private messenger: MessageService
  ) { }

  getExercicios(id: number): Observable<FuncionarioExercicios | null> {
    this._loading$.next(true);

    return this.http.get<FuncionarioExercicios | null>(`api/ferias/funcionario?Id=${id}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.messenger.notify({
            title: 'Funcionário não encontrado',
            type: MessageTypes.error
          });
          return of(null);
        }),
        finalize(() => this._loading$.next(false))
      );
  }

  getExercicioById(funcionarioId: number, exercicioId: number): Observable<Exercicio | null> {
    this._loading$.next(true);

    return this.http.get<Exercicio | null>(`api/ferias/funcionario/${funcionarioId}/exercicio/${exercicioId}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.messenger.notify({
            title: 'Houve um erro ao buscar exercício',
            type: MessageTypes.error
          });
          return of(null);
        }),
        finalize(() => this._loading$.next(false))
      );
  }

  addExercicio(newExercicio: NewExercicio, funcionarioId: number): Observable<Exercicio | null> {
    return this.http.post<Exercicio>(
      `api/ferias/funcionario/${funcionarioId}`,
      newExercicio
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorBody: ExercicioUnprocessableEntityResponse = error.error;
        let errorDesc: string[] = [];

        if (error.status < 500) {
          for (const prop in errorBody.errors) {
            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof ExercicioBackendErrors]?.[0]}`);
          }
        }

        let msg = {
          title: `Houve um erro ao adicionar exercício.`,
          type: MessageTypes.error,
          errors: (error.status < 500) ? errorDesc : [''],
        };

        this.messenger.notify(msg);
        
        return of(null);
      }),
      tap((res) => {
        if (res) {
          let msg = {
            title: `Exercício Adicionado.`,
            type: MessageTypes.success,
          };

          this.messenger.notify(msg);
        }
      })
    );
  }

  updateExercicio(updatedExercicio: UpdatedExercicio, funcionarioId: number, exercicioId: number): Observable<Exercicio | null> {
    return this.http.put<Exercicio>(
      `api/ferias/funcionario/${funcionarioId}/exercicio/${exercicioId}`,
      updatedExercicio
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorBody: ExercicioUnprocessableEntityResponse = error.error;
        let errorDesc: string[] = [];

        if (error.status < 500) {
          for (const prop in errorBody.errors) {
            errorDesc.push(`${prop}: ${errorBody.errors[prop as keyof ExercicioBackendErrors]?.[0]}`);
          }
        }

        let msg = {
          title: `Houve um erro ao editar exercício.`,
          type: MessageTypes.error,
          errors: (error.status < 500) ? errorDesc : [''],
        };

        this.messenger.notify(msg);
        
        return of(null);
      }),
      tap((res) => {
        if (res) {
          let msg = {
            title: `Exercício Atualizado.`,
            type: MessageTypes.success,
          };

          this.messenger.notify(msg);
        }
      })
    );
  }

  deleteExercicio(funcionarioId: number, exercicioId: number) {
    return this.http.delete<Exercicio>(`api/ferias/funcionario/${funcionarioId}/exercicio/${exercicioId}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          let errorBody: FuncionarioNotFoundResponse = error.error;
  
          let msg = {
            title: `Houve um erro ao deletar exercício.`,
            type: MessageTypes.error
          };
  
          this.messenger.notify(msg);
          
          return of(null);
        }),
        tap((res) => {
          if (res) {
            let msg = {
              title: `Exercício Deletado.`,
              type: MessageTypes.success,
            };
  
            this.messenger.notify(msg);
          }
        })
      );
  }
}
