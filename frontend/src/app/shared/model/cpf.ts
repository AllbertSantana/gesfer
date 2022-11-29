export class Cpf {
    public primeirosTresDigitos: string;
    public segundosTresDigitos: string;
    public terceirosTresDigitos: string;
    public ultimosDoisDigitos: string;
    
    constructor(
        primeirosTresDigitos: string,
        segundosTresDigitos: string,
        terceirosTresDigitos: string,
        ultimosDoisDigitos: string,
    ) {
        this.primeirosTresDigitos = primeirosTresDigitos;
        this.segundosTresDigitos = segundosTresDigitos;
        this.terceirosTresDigitos = terceirosTresDigitos;
        this.ultimosDoisDigitos = ultimosDoisDigitos;
    }

    toString(): string {
        return `${this.primeirosTresDigitos}.${this.segundosTresDigitos}.${this.terceirosTresDigitos}-${this.ultimosDoisDigitos}`;
    }

    static fromString(string: string): Cpf {
        return new Cpf(string.slice(0, 3), string.slice(4, 7), string.slice(8, 11), string.slice(12, 14));
    }
}