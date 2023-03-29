export interface Usuario {
    id: number;
    nome: string;
    cpf: string;
    email: string;
}

export interface UsuarioDetailed {
    id: number;
    nome: string;
    cpf: string;
    email: string;
    perfil: string;
    permissoes: string[];
}

export interface UsuarioFormValue {
    id: number;
    nome: string;
    cpf: string;
    email: string;
    perfil: string;
    senha: string;
}

export interface UsuarioResponse {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    rowCount: number;
    items: Usuario[]
}

export interface UsuarioRequestParams {
    PageNumber: string;
    PageSize: string;
    OrderBy: string;
    Order: string;
    'Filter.nome'?: string;
    'Filter.cpf'?: string;
    'Filter.email'?: string;
    'Filter.perfil'?: string;
}

export interface UsuarioBackendErrors {
    Nome?: string[];
    Usuario?: string[];
}

export interface UsuarioUnprocessableEntityResponse {
    type: string;
    title: string;
    status: number;
    traceId: string;
    errors: UsuarioBackendErrors;
}

export interface UsuarioNotFoundResponse {
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
    email: FilterState;
    perfil: FilterState;
}

export interface FiltersValues {
    nome: string;
    cpf: string;
    email: string;
    perfil: string;
}