using AutoMapper;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Exercicio // Período Aquisitivo
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public ICollection<Ferias>? Ferias { get; set; }

        public int FuncionarioId { get; set; }
        public Funcionario Funcionario { get; set; } = null!;
        //public float DiasConcedidos => (DataFim.DayNumber - DataInicio.DayNumber) * 30 / 365;
        //public int DiasUsufruidos => Ferias?.Sum(x => x.DataFim.DayNumber - x.DataInicio.DayNumber) ?? 0;
    }

    public class Ferias // Período de Gozo
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }

        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; } = null!;
    }
    
    public class ExercicioRow
    {
        public int Id { get; set; }

        [Required]
        public DateOnly DataInicio { get; set; }

        [Required]
        public DateOnly DataFim { get; set; }
    }

    public class FeriasRow
    {
        public int Id { get; set; }

        [Required]
        public DateOnly DataInicio { get; set; }

        [Required]
        public DateOnly DataFim { get; set; }
    }

    public class FeriasGroupByExercicio : ExercicioRow
    {
        public ICollection<FeriasRow>? Ferias { get; set; }
        public float DiasConcedidos => (DataFim.DayNumber - DataInicio.DayNumber) * 30 / 365;
        public int DiasUsufruidos => Ferias?.Sum(x => x.DataFim.DayNumber - x.DataInicio.DayNumber) ?? 0;
    }

    public class ExercicioForm : ExercicioRow
    {
        public ICollection<FeriasForm>? Ferias { get; set; }
        public int FuncionarioId { get; set; }
    }

    public class FeriasForm : FeriasRow { }

    public class FeriasProfile : Profile
    {
        public FeriasProfile()
        {
            CreateMap<ExercicioForm, Exercicio>();
            CreateMap<Exercicio, ExercicioRow>();
            CreateMap<Exercicio, FeriasGroupByExercicio>();

            CreateMap<FeriasForm, Ferias>();
            CreateMap<Ferias, FeriasRow>();
        }
    }

    public class FeriasQuery : QueryRequest<FeriasFilter, FeriasSortableColumn> { }
    public class FeriasResult : QueryResult<FeriasGroupByExercicio> { }

    public class FeriasFilter
    {
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
    }

    public enum FeriasSortableColumn { DataInicio }

    public class ExercicioFormValidator : AbstractValidator<ExercicioForm>
    {
        public ExercicioFormValidator()//(Funcionario? funcionario = null)
        {
            //RuleFor(x => x.FuncionarioId).NotEmpty().WithMessage("Id do funcionário é obrigatório.");

            var dataInicioRule = RuleFor(x => x.DataInicio).Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Data inicial não pode ser posterior à atual");
            //if (funcionario != null)
            //    dataInicioRule.GreaterThanOrEqualTo(x => funcionario.DataVinculo).WithMessage("Data inicial não pode ser anterior ao vínculo");

            RuleFor(x => x.DataFim).Cascade(CascadeMode.Stop)
                .GreaterThan(x => x.DataInicio).WithMessage("Data final não pode ser anterior ou igual à inicial")
                .LessThanOrEqualTo(x => x.DataInicio.AddYears(1)).WithMessage("Período aquisitivo não pode ser superior a 1 ano");
            
            RuleFor(x => x).Must(x => (x.Ferias?.Sum(i => i.DataFim.DayNumber - i.DataInicio.DayNumber) ?? 0) <= ((x.DataFim.DayNumber - x.DataInicio.DayNumber) * 30 / 365))
                .WithName("Ferias")
                .WithMessage("Dias de férias usufruídos não pode ser superior aos concedidos");
            
            RuleForEach(x => x.Ferias).SetValidator(x => new FeriasFormValidator(x));
        }
    }

    public class FeriasFormValidator : AbstractValidator<FeriasForm>
    {
        public FeriasFormValidator(ExercicioForm? exercicio = null)
        {
            var dataInicioRule = RuleFor(x => x.DataInicio).Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Data inicial não pode ser posterior à atual");
            
            RuleFor(x => x.DataFim).GreaterThan(x => x.DataInicio).WithMessage("Data final não pode ser anterior ou igual à inicial");

            if (exercicio != null)
            {
                dataInicioRule.GreaterThan(x => exercicio.DataFim).WithMessage("Data inicial não pode ser anterior ou igual ao final do exercício");

                RuleFor(a => a).Must(a =>
                    exercicio.Ferias?
                        .Where(b => !b.Equals(a))
                        .All(b => a.DataInicio > b.DataFim || a.DataFim < b.DataInicio)
                    ?? true)
                    .WithMessage("Período de férias não pode se sobrepor a outro");
            }
        }
    }

    public class FeriasFilterValidator : AbstractValidator<FeriasFilter>
    {
        public FeriasFilterValidator()// ERROR: NOT WORKING
        {
            RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.Cpf) || !string.IsNullOrEmpty(x.Matricula))
                .WithName("Funcionario")
                .WithMessage("CPF ou matrícula deve ser fornecido");
        }
    }
}
