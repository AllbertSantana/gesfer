import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, finalize, Observable, Subject, takeUntil } from "rxjs";
import { FiltersValues, Usuario } from "../model/usuario";
import { UsuarioService } from "../usuario.service";

export class UsuarioDataSource implements DataSource<Usuario> {
    disconnected$ = new Subject<void>;

    private usuariosSubject$ = new BehaviorSubject<Usuario[]>([]);

    constructor(private usuarioService: UsuarioService) {}

    connect(collectionViewer: CollectionViewer): Observable<Usuario[]> {
        return this.usuariosSubject$.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.usuariosSubject$.complete();
        
        this.disconnected$.next();
        this.disconnected$.complete();
    }

    loadUsuarios(
        pageNumber = 0,
        pageSize = 5,
        orderBy = 'nome',
        order = 'asc',
        filters: FiltersValues = {nome: '', cpf: '', email: '', perfil: ''}
      ) {
        this.usuarioService
            .getUsuarios(pageNumber, pageSize, filters, orderBy, order)
            .pipe(takeUntil(this.disconnected$))
            .subscribe(
                (usuarios) => this.usuariosSubject$.next(usuarios)
            );
    }
}