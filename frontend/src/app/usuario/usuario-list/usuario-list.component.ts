import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { RequestService } from '../request.service';
import { FiltersValues, Usuario } from '../model/usuario';
import { BehaviorSubject, Observable, Subject, merge, take, takeUntil, tap } from 'rxjs';
import { UsuarioDataSource } from '../implementations/usuario-data-source';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { RemoveDialogComponent } from 'src/app/shared/components/remove/remove-dialog.component';
import { Router } from '@angular/router';
import { FilterComponent } from 'src/app/shared/components/filter/filter.component';
import { initialInputsConfigs } from '../input-configs';
import { ListService } from '../list.service';
import { Filter, SortDirection, SortField } from 'src/app/shared/model/list-property';
import { FormType, InputConfig } from 'src/app/shared/model/form';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css']
})
export class UsuarioListComponent implements OnInit, AfterViewInit, OnDestroy {
  private _destroyed$ = new Subject<void>();
  public dataSource: UsuarioDataSource;
  public displayedColumns = ['cpf', 'nome', 'email'];
  public initialPaginator = this.listService.getPaginator();
  public initialSort = this.listService.getSort();
  public selectedUsuario$ = new BehaviorSubject<Usuario | undefined>(undefined);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private _router: Router,
    public requestService: RequestService,
    public listService: ListService,
    public authService: AuthService,
    public dialog: MatDialog,
  ) {
    this.dataSource = new UsuarioDataSource(this.requestService);
  }

  ngOnInit(): void {
    this.listService.getFiltersAsObservable()
      .pipe(takeUntil(this._destroyed$))
      .subscribe(
        latestFilters => {
          let filterValues: FiltersValues = {
            nome: latestFilters.filter(f => f.name === 'nome')[0].value.toString(),
            cpf: latestFilters.filter(f => f.name === 'cpf')[0].value.toString(),
            email: latestFilters.filter(f => f.name === 'email')[0].value.toString(),
            perfil: latestFilters.filter(f => f.name === 'perfil')[0].value.toString(),
          };

          this.dataSource.loadUsuarios(
            this.initialPaginator.pageIndex,
            this.initialPaginator.pageSize,
            this.initialSort.active,
            this.initialSort.direction,
            filterValues
          );
        }
      )
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.paginator.page, this.sort.sortChange)
      .pipe(
        takeUntil(this._destroyed$),
        tap(
          () => {
            let latestFilters: Filter[] = this.listService.getFilters();
            let filterValues: FiltersValues = {
              nome: latestFilters.filter(f => f.name === 'nome')[0].value.toString(),
              cpf: latestFilters.filter(f => f.name === 'cpf')[0].value.toString(),
              email: latestFilters.filter(f => f.name === 'email')[0].value.toString(),
              perfil: latestFilters.filter(f => f.name === 'perfil')[0].value.toString(),
            };

            this.dataSource.loadUsuarios(
              this.paginator.pageIndex + 1,
              this.paginator.pageSize,
              this.sort.active,
              this.sort.direction,
              filterValues
            );
          }
        )
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.listService.updatePaginator(this.paginator.pageIndex + 1, this.paginator.pageSize);
    this.listService.updateSort({ direction: this.sort.direction as SortDirection, active: this.sort.active as SortField})

    this._destroyed$.next();
    this._destroyed$.complete();
  }

  getFilterConfig(): InputConfig[] {
    let latestFilters: Filter[] = this.listService.getFilters();
    let latestFilterConfig: InputConfig[] = initialInputsConfigs
      .filter(config => config.control.usedAt.some(form => form === FormType.filter))
      .map(
        (input, index) => {
          return {
            control: {
              ...latestFilters[index],
              isDisabled: !latestFilters[index].value,
              usedAt: input.control.usedAt,
              validators: input.control.validators
            },
            view: input.view
          }
        }
      );

    return latestFilterConfig;
  }

  openFilterDialog() {
    const filterDialogRef = this.dialog.open(FilterComponent, {
      width: '500px',
      height: '551px',
      data: this.getFilterConfig()
    });

    filterDialogRef.afterClosed()
      .pipe(take(1))
      .subscribe(
        (submittedFilters: Filter[]) => {
          if(submittedFilters) {
            let isFilterEnabled: boolean = submittedFilters.some(filter => !!filter.value);
            isFilterEnabled ? this.listService.setFilterAbilityAs(true) : this.listService.setFilterAbilityAs(false)
            this.listService.updateFilters(submittedFilters);
          }
        }
      );
  }

  openRemoveDialog(): void {
    let usuario: Usuario = this.selectedUsuario$.value!;
    const removeDialogRef = this.dialog.open(RemoveDialogComponent, {data: {usuario: usuario}});
    
    removeDialogRef.afterClosed()
      .pipe(take(1))
      .subscribe(
        (proceeded: boolean) => {
          if (proceeded) {
            this.requestService.removeUsuario(usuario)
              .pipe(take(1))
              .subscribe(
                (_) => {
                  if (this.selectedUsuario$.value!.cpf === this.authService.userInfo?.cpf) {
                    this.authService.logout();
                    this._router.navigateByUrl('login');
                  } else {
                    this.dataSource.loadUsuarios();
                  }
                }
              )
          }
        }
      );
  }
}