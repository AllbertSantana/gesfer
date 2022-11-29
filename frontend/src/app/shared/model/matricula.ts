export class Matricula {
    public idDigitos: string;
    public vinculoDigitos: string;
    
    constructor(
        idDigitos: string,
        vinculoDigitos: string,
    ) {
        this.idDigitos = idDigitos;
        this.vinculoDigitos = vinculoDigitos;
    }

    toString(): string {
        return `${this.idDigitos}/${this.vinculoDigitos}`;
    }

    static fromString(string: string): Matricula {
        let parts = string.split('/');

        return new Matricula(parts[0], parts[1]);
    }
}