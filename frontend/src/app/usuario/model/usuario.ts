// extending a interface with this IObjectKeys interface allows me to index it with a string; ex: obj['string'];
// more information why i am using this IObjectKeys interface: https://stackoverflow.com/a/64282168;
interface IObjectKeys {
    [key: string]: string | string[] | number;
}

export interface Usuario {
    id: number;
    nome: string;
    cpf: string;
    email: string;
}

export interface UsuarioDetailed extends IObjectKeys  {
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