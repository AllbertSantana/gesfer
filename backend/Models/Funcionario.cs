using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome vazio.")]
        [StringLength(100, ErrorMessage = "Nome superior à 100 caractéres.")]
        public string Nome { get; set; } = null!;
        
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido.")]
        public string Cpf { get; set; } = null!;

        [RegularExpression(@"^\d{4,10}\/\d{1,2}$", ErrorMessage = "Matrícula inválida.")]
        public string Matricula { get; set; } = null!;

        [IsTodayOrBefore(ErrorMessage = "Data de vínculo superior a hoje.")]
        public DateOnly DataVinculo { get; set; } // Início das Atividades
    }

    public class PlanilhaFuncionario : QueryResults<Funcionario> { }
    public class ConsultaFuncionario : QueryArguments<FiltroFuncionario, ColunaFuncionario> { }

    public class FiltroFuncionario
    {
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
    }

    public enum ColunaFuncionario { Id, Nome, Cpf, Matricula, DataVinculo }

    public class IsTodayOrBefore : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return (DateOnly.FromDateTime(DateTime.Today).CompareTo(value) >= 0);
        }
    }
}
