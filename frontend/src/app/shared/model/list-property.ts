export enum SortField {
    nome = "nome",
    cpf = "cpf",
    email = "email",
}

export enum SortDirection {
    asc = "asc",
    desc = "desc",
}

export interface Filter {
    name: string;
	value: string | number | boolean;
}

export interface Paginator {
    pageIndex: number;
    pageSize: number;
    pageSizeOptions: number[];
}

export interface Sort {
    active: SortField;
    direction: SortDirection;
}