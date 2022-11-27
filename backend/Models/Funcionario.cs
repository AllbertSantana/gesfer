using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace backend.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [StringLength(14)]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")]
        public string Cpf { get; set; } = null!;

        [Required]
        [StringLength(13)]
        [RegularExpression(@"^\d{4,10}\/\d{1,2}$")]
        public string Matricula { get; set; } = null!;

        [Required]
        public DateOnly DataVinculo { get; set; }

        public ICollection<Exercicio>? Exercicios { get; set; }
    }

    public class FuncionarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Matricula { get; set; } = null!;
        public DateOnly DataVinculo { get; set; }
        public ICollection<ExercicioDto>? Exercicios { get; set; }

        public float SaldoDias => Exercicios?.Sum(x => x.DiasConcedidos - x.DiasUsufruidos) ?? 0;
    }

    public class ConsultaFuncionario : QueryArguments<FiltroFuncionario, ColunaFuncionario> { }
    public class PlanilhaFuncionario : QueryResults<Funcionario> { }
    
    public class FiltroFuncionario
    {
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
    }

    public enum ColunaFuncionario { Id, Nome, Cpf, Matricula, DataVinculo }

    public class FuncionarioValidator : AbstractValidator<Funcionario>
    {
        public FuncionarioValidator()
        {
            RuleFor(x => x.Nome).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome não pode ultrapassar 100 caractéres");
            
            RuleFor(x => x.Cpf).Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF não atende ao padrão");
            
            RuleFor(x => x.Matricula).Matches(@"^\d{4,10}\/\d{1,2}$").WithMessage("Matrícula não atende ao padrão");
            
            RuleFor(x => x.DataVinculo).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Data de vínculo não pode ser posterior à atual");
            
            RuleForEach(x => x.Exercicios).SetValidator(x => new ExercicioValidator(x));
        }
    }
}
