import { Injectable } from "@angular/core";
import { Filter, Paginator, Sort, SortDirection, SortField } from "../shared/model/list-property";
import { BehaviorSubject, Observable } from "rxjs";
import { initialInputsConfigs } from "./input-configs";

@Injectable({
    providedIn: 'root',
})
export class ListService {
    private _initialFilterValue: Filter[] = initialInputsConfigs.map(filter => {
        return {
            name: filter.control.name,
            value: filter.control.value,
            isDisabled: filter.control.isDisabled
        }
    });
    private _filters$ = new BehaviorSubject<Filter[]>(this._initialFilterValue);
    private _isFilterEnabled$ = new BehaviorSubject<boolean>(false);
    private _paginator: Paginator = { pageIndex: 1, pageSize: 5, pageSizeOptions: [5, 10, 15, 20] };
    private _sort: Sort = { active: SortField.nome, direction: SortDirection.asc };

    constructor() {}

    getFiltersAsObservable(): Observable<Filter[]> { return this._filters$.asObservable(); }
    getFilters(): Filter[] { return this._filters$.value; }
    updateFilters(newFilter: Filter[]): void { this._filters$.next(newFilter); }
    clearFilters(): void { this._filters$.next(this._initialFilterValue); }
    isFilterEnabled(): Observable<boolean> { return this._isFilterEnabled$.asObservable(); }
    setFilterAbilityAs(isActive: boolean): void { this._isFilterEnabled$.next(isActive); }

    getPaginator(): Paginator { return this._paginator; }
    updatePaginator(pageIndex: number, pageSize: number): void { this._paginator = { ...this._paginator, pageIndex: pageIndex, pageSize: pageSize } }
    clearPaginator(): void { this._paginator = { pageIndex: 1, pageSize: 5, pageSizeOptions: [5, 10, 15, 20] }; }

    getSort(): Sort { return this._sort; }
    updateSort(sort: Sort): void { this._sort = sort; }
    clearSort(): void { this._sort = { active: SortField.nome, direction: SortDirection.asc }; }
}