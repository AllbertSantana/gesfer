export interface Usuario {
    id: number;
    nome: string;
    cpf: string;
    email: string;
    perfil: string;
    permissoes: string[];
}

export interface JwtPayload {
    Id: string;
    Perfil: string;
    nbf: Date;
    exp: Date;
    iat: Date;
}

export interface Login {
    cpf: string;
    email: string;
    senha: string;
}