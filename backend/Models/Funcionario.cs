﻿using AutoMapper;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace backend.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Matricula { get; set; } = null!;
        
        public ICollection<Exercicio>? Exercicios { get; set; }
    }
    
    public class FuncionarioRow
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")]
        public string Cpf { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{4,10}\/\d{1,2}$")]
        public string Matricula { get; set; } = null!;
    }

    public class FuncionarioSpreadsheetRow
    {
        public string? Nome { get; set; }

        [Display(Name = "CPF")]
        public string? Cpf { get; set; }

        [Display(Name = "Matrícula")]
        public string? Matricula { get; set; }
    }

    public class FuncionarioForm : FuncionarioRow { }

    public class FuncionarioProfile : Profile
    {
        public FuncionarioProfile()
        {
            CreateMap<FuncionarioForm, Funcionario>();
            CreateMap<Funcionario, FuncionarioRow>();
            CreateMap<FuncionarioRow, FuncionarioSpreadsheetRow>();
        }
    }

    public class FuncionarioQuery : QueryRequest<FuncionarioFilter, FuncionarioSortableColumn> { }
    public class FuncionarioResult : QueryResult<FuncionarioRow> { }
    
    public class FuncionarioFilter
    {
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
    }

    public enum FuncionarioSortableColumn { Id, Nome, Cpf, Matricula }//, DataVinculo

    public class FuncionarioFormValidator : AbstractValidator<FuncionarioForm>
    {
        public FuncionarioFormValidator()
        {
            RuleFor(x => x.Nome).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome não pode ultrapassar 100 caracteres");
            
            RuleFor(x => x.Cpf).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF não atende ao padrão");
            
            RuleFor(x => x.Matricula).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Matrícula é obrigatória")
                .Matches(@"^\d{4,10}\/\d{1,2}$").WithMessage("Matrícula não atende ao padrão");
            
            //RuleFor(x => x.DataVinculo).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Data de vínculo não pode ser posterior à atual");
            
            //RuleForEach(x => x.Exercicios).SetValidator(x => new ExercicioValidator(x));
        }
    }
}
