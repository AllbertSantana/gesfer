using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface IFuncionarioService
    {
        public Funcionario? Read(int id);
        public IEnumerable<Funcionario> Read(int pageNumber, int pageSize, string? matricula, string? cpf, string? nome);
        public int CreateOrUpdate(Funcionario funcionario);
        public void Delete(int id);
    }

    public class FuncionarioService : IFuncionarioService
    {
        private readonly GestaoDbContext _context;

        public FuncionarioService(GestaoDbContext context)
        {
            _context = context;
        }

        public Funcionario? Read(int id)
            => _context.Funcionarios.Find(id);

        public IEnumerable<Funcionario> Read(int pageNumber, int pageSize, string? matricula, string? cpf, string? nome)
        {
            var funcionarios = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrEmpty(matricula))
                funcionarios = funcionarios.Where(f => f.Matricula == matricula);
            else if (!string.IsNullOrEmpty(cpf))
                funcionarios = funcionarios.Where(f => f.Cpf == cpf);
            else if (!string.IsNullOrEmpty(nome))
                funcionarios = funcionarios.Where(f => f.Nome.Contains(nome));

            return funcionarios.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        
        public int CreateOrUpdate(Funcionario funcionario)
        {
            if (_context.Funcionarios.Any(f => f.Id != funcionario.Id && (f.Matricula == funcionario.Matricula || f.Cpf == funcionario.Cpf)))
                throw new BadHttpRequestException("Matrícula ou CPF em uso.");

            _context.Funcionarios.Update(funcionario);
            _context.SaveChanges();
            return funcionario.Id;
        }

        public void Delete(int id)
        {
            _context.Funcionarios.Remove(new() { Id = id });
            _context.SaveChanges();
        }
    }
}
