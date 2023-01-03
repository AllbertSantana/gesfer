using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public Perfil Perfil { get; set; }
    }

    public static class Papel
    {
        public static readonly Dictionary<Perfil, List<Permissao>> Permissoes;

        static Papel()
        {
            Permissoes = new();
            Permissoes[Perfil.Consultor] = new() { Permissao.BuscarFuncionario, Permissao.ListarFuncionarios, Permissao.BaixarPlanilhaFuncionarios, Permissao.BuscarFerias, Permissao.ListarFerias, Permissao.BaixarPlanilhaFerias };
            Permissoes[Perfil.Cadastrante] = Permissoes[Perfil.Consultor].Concat(new[] { Permissao.RemoverFuncionario, Permissao.RemoverFuncionarios, Permissao.CriarFuncionario, Permissao.EditarFuncionario, Permissao.RemoverFerias, Permissao.CriarFerias, Permissao.EditarFerias }).ToList();
            Permissoes[Perfil.Administrador] = Permissoes[Perfil.Cadastrante].Concat(new[] { Permissao.BuscarUsuario, Permissao.ListarUsuarios, Permissao.RemoverUsuario, Permissao.CriarUsuario, Permissao.EditarUsuario }).ToList();
        }
    }

    public enum Perfil
    {
        Consultor = 1,
        Cadastrante,
        Administrador
    }

    public enum Permissao
    {
        //Consultor
        BuscarFuncionario = 1,
        ListarFuncionarios,
        BaixarPlanilhaFuncionarios,
        BuscarFerias,
        ListarFerias,
        BaixarPlanilhaFerias,
        //Cadastrante
        RemoverFuncionario,
        RemoverFuncionarios,
        CriarFuncionario,
        EditarFuncionario,
        RemoverFerias,
        CriarFerias,
        EditarFerias,
        //Administrador
        BuscarUsuario,
        ListarUsuarios,
        RemoverUsuario,
        CriarUsuario,
        EditarUsuario
    }

    public class UsuarioSummary
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")]
        public string Cpf { get; set; } = null!;

        [Required]
        [EmailAddress]
        [RegularExpression(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$")]
        public string Email { get; set; } = null!;
    }

    public class UsuarioAccount : UsuarioSummary
    {
        [Required]
        public Perfil Perfil { get; set; }

        public List<Permissao> Permissoes => Papel.Permissoes.ContainsKey(Perfil) ? Papel.Permissoes[Perfil] : new();
    }

    [SwaggerSchema("Senha e Perfil são obrigatórios no cadastro do usuário.<br>Perfil é ignorado na atualização cadastral do usuário logado.")]
    public class SignupForm : UsuarioSummary// formulário de registro ou atualização cadastral
    {
        public Perfil Perfil { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(16, MinimumLength = 8)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*_+=-]).*$")]
        public string Senha { get; set; } = null!;// nova senha
        //[Required]
        //public string SenhaAtual { get; set; } = null!;// senha do executor (requer Id do usuário executor)
    }
    
    public class LoginForm
    {
        public string? Cpf { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Senha { get; set; } = null!;
    }

    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            //CreateMap<UsuarioForm, Usuario>();
            //CreateMap<Usuario, UsuarioRow>();
            CreateMap<SignupForm, Usuario>();
            CreateMap<Usuario, UsuarioSummary>();
            CreateMap<Usuario, UsuarioAccount>();
        }
    }

    public class UsuarioQuery : QueryRequest<UsuarioFilter, UsuarioSortableColumn> { }
    public class UsuarioResult : QueryResult<UsuarioSummary> { }

    public class UsuarioFilter
    {
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Email { get; set; }
        public Perfil? Perfil { get; set; }
    }

    public enum UsuarioSortableColumn { Id, Nome, Cpf, Email, Perfil }

    public class SignupFormValidator : AbstractValidator<SignupForm>
    {
        public SignupFormValidator()
        {
            RuleFor(x => x.Nome).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome não pode ultrapassar 100 caracteres");

            RuleFor(x => x.Cpf).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF não atende ao padrão");

            RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail não atende ao padrão");

            RuleFor(x => x.Perfil)
                .IsInEnum().WithMessage("Perfil inválido")
                .When(x => x.Id <= 0 || x.Perfil != default);// "Perfil é obrigatório para o cadastro"

            RuleFor(x => x.Senha).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Senha é obrigatória para o cadastro")
                .Matches(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*_+=-]).*$")
                .WithMessage("Senha deve ter no mínimo 8 caracteres, contendo pelo menos uma letra maiúscula (A-Z), uma letra minúscula (a-z), um número (0-9) e um caractere especial (!@#$%&*_-+=)")
                .When(x => x.Id <= 0 || !string.IsNullOrWhiteSpace(x.Senha));
        }
    }

    public class LoginFormValidator : AbstractValidator<LoginForm>
    {
        public LoginFormValidator()
        {
            RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha é obrigatória");

            RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Cpf) || !string.IsNullOrWhiteSpace(x.Email))
                .WithName(nameof(Usuario))
                .WithMessage("CPF ou e-mail deve ser fornecido");
        }
    }
}
