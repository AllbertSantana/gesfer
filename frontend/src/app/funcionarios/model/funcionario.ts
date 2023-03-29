export interface Funcionario {
    id: number;
    nome: string;
    cpf: string;
    matricula: string,
}

export interface FuncionarioResponse {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    rowCount: number;
    items: Funcionario[]
}

export interface FuncionarioRequestParams {
    PageNumber: string;
    PageSize: string;
    OrderBy: string;
    Order: string;
    'Filter.nome'?: string;
    'Filter.cpf'?: string;
    'Filter.matricula'?: string;
}

export interface FuncionarioBackendErrors {
    Nome?: string[];
    Funcionario?: string[];
}

export interface FuncionarioUnprocessableEntityResponse {
    type: string;
    title: string;
    status: number;
    traceId: string;
    errors: FuncionarioBackendErrors;
}

export interface FuncionarioNotFoundResponse {
    type: string;
    title: string;
    status: number;
    traceId: string;
    detail: string;
}

export interface FilterState {
    value: string;
    disabled: boolean;
}

export interface Filters {
    nome: FilterState;
    cpf: FilterState;
    matricula: FilterState;
}

export interface FiltersValues {
    nome: string;
    cpf: string;
    matricula: string;
}