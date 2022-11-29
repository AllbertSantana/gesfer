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

  getFuncionarios(
    pageNumber = 1,
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
      tap(res => {this._length$.next(res.rowCount)}),
      map(res => res.items)
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