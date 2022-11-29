import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, catchError, finalize, Observable, of, Subject, takeUntil } from "rxjs";
import { FuncionariosService } from "../funcionarios.service";
import { Filters, Funcionario } from "../model/funcionario";

export class FuncionariosDataSource implements DataSource<Funcionario> {
    disconnected$ = new Subject<void>;

    private funcionariosSubject = new BehaviorSubject<Funcionario[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);
    

    public loading$ = this.loadingSubject.asObservable();
    

    constructor(private funcionariosService: FuncionariosService) {}

    connect(collectionViewer: CollectionViewer): Observable<Funcionario[]> {
        return this.funcionariosSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.funcionariosSubject.complete();
        this.loadingSubject.complete();
        
        this.disconnected$.next();
        this.disconnected$.complete();
    }

    loadFuncionarios(
        pageNumber = 0,
        pageSize = 5,
        orderBy = 'nome',
        order = 'asc',
        filters: Filters = {Nome: '', Cpf: '', Matricula: ''}
      ) {
        this.loadingSubject.next(true);

        this.funcionariosService.getFuncionarios(pageNumber, pageSize, filters, orderBy, order)
            .pipe(
                takeUntil(this.disconnected$),
                catchError(() => of([])),
                finalize(() => this.loadingSubject.next(false))
            )
            .subscribe(
                (funcionarios) => this.funcionariosSubject.next(funcionarios)
            );
    }
}