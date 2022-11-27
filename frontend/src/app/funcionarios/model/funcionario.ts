export interface Funcionario {
    id: number;
    nome: string;
    cpf: string;
    matricula: string,
    dataVinculo: string
}

export interface Response {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    rowCount: number;
    results: Funcionario[]
}

export interface Filters {
    Nome: string;
    Cpf: string;
    Matricula: string;
}
