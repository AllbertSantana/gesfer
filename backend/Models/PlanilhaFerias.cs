﻿namespace backend.Models
{
    public class PlanilhaFerias : QueryResults<FuncionarioDto> { }
    public class ConsultaFerias : QueryArguments<FiltroFerias> { }

    public class FiltroFerias
    {
        public string? Nome { get; set; }
        public string? Matricula { get; set; }
        public float? Saldo { get; set; }
        public DateOnly? InicioExercicio { get; set; }
        public DateOnly? FimExercicio { get; set; }
        public DateOnly? InicioFerias { get; set; }
        public DateOnly? FimFerias { get; set; }
    }

    public class FuncionarioDto
    {
        public uint Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Matricula { get; set; } = null!;
        public DateOnly DataVinculo { get; set; }
        public float Saldo { get; set; }
        public IEnumerable<ExercicioDto>? Exercicios { get; set; }
    }

    public class ExercicioDto
    {
        public uint Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public float Saldo { get; set; }
        public IEnumerable<FeriasDto>? Ferias { get; set; }
    }

    public class FeriasDto
    {
        public uint Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}
