import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, Observable, pipe, Subject, takeUntil, tap } from 'rxjs';
import { FuncionarioFilterDialogComponent } from '../funcionario-filter/funcionario-filter-dialog.component';
import { RemoveDialogComponent } from '../remove/remove-dialog.component';
import { FuncionariosService } from '../funcionarios.service';
import { FuncionariosDataSource } from '../implementations/funcionarios-data-source';
import { Filters, FiltersValues, Funcionario } from '../model/funcionario';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-funcionario-list',
  templateUrl: './funcionario-list.component.html',
  styleUrls: ['./funcionario-list.component.css']
})
export class FuncionarioListComponent implements OnInit, AfterViewInit, OnDestroy {
  destroyed$ = new Subject<void>;
  funcionariosLength$: Observable<number>;
  filtersValues!: FiltersValues;
  hasFilter!: boolean;

  dataSource: FuncionariosDataSource;
  displayedColumns = ['matricula', 'cpf', 'nome'];
  selectedRows: Map<number, Funcionario> = new Map<number, Funcionario>;
  pageSize = 5;
  pageSizeOptions = [5, 10, 15, 20];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private funcionariosService: FuncionariosService,
    public authService: AuthService,
    public dialog: MatDialog,
  ) {
    this.dataSource = new FuncionariosDataSource(this.funcionariosService);
    this.funcionariosLength$ = this.funcionariosService.getRowCount();
  }

  ngOnInit(): void {
    this.loadFilters();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
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
    this.dataSource.loadFuncionarios(
      this.paginator ? this.paginator.pageIndex + 1 : 1,
      this.paginator ? this.paginator.pageSize : this.pageSize,
      this.sort ? this.sort.active : 'nome',
      this.sort ? this.sort.direction : 'asc',
      this.filtersValues
    );
  }

  loadFilters() {
    this.funcionariosService.getFilters()
      .pipe(
        takeUntil(this.destroyed$)
      )
      .subscribe(
        filters => {
          let filtersValues: FiltersValues = {nome: filters.nome.value, cpf: filters.cpf.value, matricula: filters.matricula.value};
          this.filtersValues = filtersValues;
          this.loadDataSource();
        }
      );

    this.funcionariosService.getHasFilter()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(isFiltered => this.hasFilter = isFiltered);
  }

  openFilterDialog() {
    const filterDialogRef = this.dialog.open(FuncionarioFilterDialogComponent, {
      width: '500px',
      height: '551px'
    });

    filterDialogRef.afterClosed()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        (submittedFiltersValues: FiltersValues) => {
          if (submittedFiltersValues) {
            this.paginator.pageIndex = 0;
            //this.loadDataSource();
          }
        }
      );
  }

  openRemoveDialog(): void {
    let funcionario: Funcionario = this.getRowSelection()[0];

    const removeDialogRef = this.dialog.open(RemoveDialogComponent, {
      data: {
        funcionario: funcionario,
      },
    });

    removeDialogRef.afterClosed()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        (proceeded: boolean) => {
          if (proceeded) {
            this.funcionariosService.removeFuncionario(funcionario)
              .pipe(takeUntil(this.destroyed$))
              .subscribe(
                finalized => {
                  this.paginator.pageIndex = 0;
                  this.selectedRows.clear();
                  this.loadDataSource();
                }
            );
          }
        }
      );
  }

  onRowSelect(row: Funcionario): void {
    if (this.selectedRows.has(row.id)) {
      this.selectedRows.delete(row.id);
    } else {
      this.selectedRows.clear();
      this.selectedRows.set(row.id, row);
    }
  }

  getRowSelection(): Funcionario[] {
    var funcionario = [];
    for (let row of this.selectedRows.values()) {
      funcionario.push(row);
    }

    return funcionario;
  }
}