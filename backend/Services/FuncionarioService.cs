using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface IFuncionarioService
    {
        public ValueTask<Funcionario?> Read(int id);
        public Task<ListagemFuncionario> Read(ConsultaFuncionario filter);
        public Task<int> CreateOrUpdate(Funcionario row);
        public Task Delete(int id);
    }

    public class FuncionarioService : IFuncionarioService
    {
        private readonly GestaoDbContext _context;

        public FuncionarioService(GestaoDbContext context)
        {
            _context = context;
        }

        public ValueTask<Funcionario?> Read(int id)
            => _context.Funcionarios.FindAsync(id);

        public async Task<ListagemFuncionario> Read(ConsultaFuncionario filter)
        {
            var query = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Arguments.Matricula))
                query = query.Where(f => f.Matricula == filter.Arguments.Matricula);
            else if (!string.IsNullOrEmpty(filter.Arguments.Cpf))
                query = query.Where(f => f.Cpf == filter.Arguments.Cpf);
            else if (!string.IsNullOrEmpty(filter.Arguments.Nome))
                query = query.Where(f => f.Nome.Contains(filter.Arguments.Nome));

            var dataSet = new ListagemFuncionario();
            dataSet.PageNumber = filter.PageNumber;
            dataSet.PageSize = filter.PageSize;
            dataSet.RowCount = await query.CountAsync();
            dataSet.PageCount = (int)Math.Ceiling(dataSet.RowCount / (float)dataSet.PageSize);
            dataSet.Results = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return dataSet;
        }
        
        public async Task<int> CreateOrUpdate(Funcionario row)
        {
            var exists = await _context.Funcionarios.AnyAsync(f => f.Id != row.Id && (f.Matricula == row.Matricula || f.Cpf == row.Cpf));
            if (exists)
                throw new BadHttpRequestException("Matrícula ou CPF em uso.");

            _context.Funcionarios.Update(row);
            await _context.SaveChangesAsync();
            return row.Id;
        }

        public async Task Delete(int id)
        {
            _context.Funcionarios.Remove(new() { Id = id });
            await _context.SaveChangesAsync();
        }
    }
}
