export interface Funcionario {
    id: number;
    nome: string;
    cpf: string;
    matricula: string,
}

export interface Response {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    rowCount: number;
    items: Funcionario[]
}

export interface Filters {
    Nome: string;
    Cpf: string;
    Matricula: string;
}
