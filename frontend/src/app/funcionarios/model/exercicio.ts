export interface Periodo {
    dataInicio: string;
    dataFim: string;
}

export interface Exercicio {
    id: number;
    dataInicio: string;
    dataFim: string;
    diasConcedidos: number;
    diasUsufruidos: number;
    ferias: Periodo[]
}

export interface NewExercicio {
    dataInicio: string;
    dataFim: string;
    ferias: Periodo[]
}

export interface UpdatedExercicio {
    id: number;
    dataInicio: string;
    dataFim: string;
    ferias: Periodo[]
}

export interface FuncionarioExercicios {
    id: number;
    nome: string;
    cpf: string;
    matricula: string;
    saldoDias: number;
    exercicios: Exercicio[];
}

export interface FeriasBackendErrors {
    dataInicio?: string[];
    dataFim?: string[];
}

export interface ExercicioBackendErrors {
    dataInicio?: string[];
    dataFim?: string[];
    ferias?: FeriasBackendErrors[];
}

export interface ExercicioUnprocessableEntityResponse {
    type: string;
    title: string;
    status: number;
    traceId: string;
    errors: ExercicioBackendErrors;
}