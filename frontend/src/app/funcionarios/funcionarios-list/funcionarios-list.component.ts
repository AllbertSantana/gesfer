import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, Observable, pipe, Subject, takeUntil, tap } from 'rxjs';
import { FuncionariosService } from '../funcionarios.service';
import { FuncionariosDataSource } from '../implementations/funcionarios-data-source';
import { Funcionario } from '../model/funcionario';

@Component({
  selector: 'app-funcionarios-list',
  templateUrl: './funcionarios-list.component.html',
  styleUrls: ['./funcionarios-list.component.css']
})
export class FuncionariosListComponent implements OnInit, AfterViewInit, OnDestroy {
  destroyed$ = new Subject<void>;
  funcionariosLength$: Observable<number>;

  dataSource: FuncionariosDataSource;
  displayedColumns = ['matricula', 'cpf', 'nome', 'datavinculo'];
  selectedRows!: Map<number, Funcionario>;
  pageSize = 5;
  pageSizeOptions = [5, 10, 15, 20];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private funcionariosService: FuncionariosService,
  ) {
    this.dataSource = new FuncionariosDataSource(this.funcionariosService);
    this.funcionariosLength$ = this.funcionariosService.getRowCount();
  }

  ngOnInit(): void {
    this.dataSource.loadFuncionarios(1, this.pageSize);
    this.loadSelectedRows();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.selectedRows.clear();
    this.funcionariosService.clearSelectedRows(this.selectedRows);
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.paginator.page, this.sort.sortChange)
      .pipe(
        takeUntil(this.destroyed$),
        tap(() => this.loadDataSource())
      )
      .subscribe();
  }

  loadDataSource() {
    this.dataSource.loadFuncionarios(this.paginator.pageIndex + 1, this.paginator.pageSize, this.sort.active, this.sort.direction);
  }

  loadSelectedRows() {
    this.funcionariosService.getSelectedRows()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        selection => this.selectedRows = selection
      );
  }

  onRowClick(row: Funcionario): void {
    this.selectedRows.has(row.id)
    ? this.selectedRows.delete(row.id)
    : this.selectedRows.set(row.id, row);

    this.funcionariosService.updateSelectedRows(this.selectedRows);
  }

  getSelectedRowId(): string {
    var funcionario = [];
    for (let row of this.selectedRows.values()) {
      funcionario.push(row);
    }

    return funcionario[0].id.toString();
  }
}