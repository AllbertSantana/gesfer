import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, retry, tap, throwError } from 'rxjs';
import { Filters, Funcionario, Response } from './model/funcionario';

@Injectable({
  providedIn: 'root'
})
export class FuncionariosService {
  private _initialSelectedRows = new Map<number, Funcionario>;
  
  private _length$ = new BehaviorSubject<number>(0);
  private _selectedRows$ = new BehaviorSubject<Map<number, Funcionario>>(this._initialSelectedRows);

  constructor(private http: HttpClient) { }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
    }

    return throwError(() => new Error('Something bad happened; please try again later.'));
  }

  getFuncionarios(
    pageNumber = 0,
    pageSize = 5,
    filters: Filters = {Nome: '', Cpf: '', Matricula: ''},
    orderBy = 'nome',
    order = 'asc',
  ): Observable<Funcionario[]> {
    return this.http.get<Response>(
      '/api/funcionario',
      {
        params: new HttpParams()
          .set('PageNumber', pageNumber.toString())
          .set('PageSize', pageSize.toString())
          .set('Arguments', JSON.stringify(filters))
          .set('OrderBy', orderBy)
          .set('Order', order)
      }
    )
    .pipe(
      catchError(this.handleError),
      tap(res => this._length$.next(res.rowCount)),
      map(res => res.results)
    )
  }

  getRowCount(): Observable<number> {
    return this._length$.asObservable();
  }

  getSelectedRows(): Observable<Map<number, Funcionario>> {
    return this._selectedRows$.asObservable();
  }

  updateSelectedRows(selection: Map<number, Funcionario>): void {
    this._selectedRows$.next(selection);
  }

  clearSelectedRows(selection: Map<number, Funcionario>) {
    this._selectedRows$.next(selection);
  }
}