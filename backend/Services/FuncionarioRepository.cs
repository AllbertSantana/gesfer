using backend.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Linq.Expressions;
using System.Net;

namespace backend.Services
{
    public interface IFuncionarioRepository
    {
        Task<(HttpStatusCode, Funcionario?)> Read(int id);
        Task<(HttpStatusCode, PlanilhaFuncionario)> Read(ConsultaFuncionario filter);
        Task<(HttpStatusCode, object?)> CreateOrUpdate(Funcionario record);
        Task<(HttpStatusCode, object?)> Delete(int id);
    }

    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly GestaoDbContext _context;

        public FuncionarioRepository(GestaoDbContext context)
        {
            _context = context;
        }

        public async Task<(HttpStatusCode, Funcionario?)> Read(int id)
        {
            var record = await _context.Funcionarios.FindAsync(id);
            if (record == null)
                return (HttpStatusCode.NotFound, record);
            return (HttpStatusCode.OK, record);
        }

        public async Task<(HttpStatusCode, PlanilhaFuncionario)> Read(ConsultaFuncionario filter)
        {
            var query = _context.Funcionarios.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Arguments?.Matricula))
                query = query.Where(f => f.Matricula == filter.Arguments.Matricula);
            else if (!string.IsNullOrEmpty(filter.Arguments?.Cpf))
                query = query.Where(f => f.Cpf == filter.Arguments.Cpf);
            else if (!string.IsNullOrEmpty(filter.Arguments?.Nome))
                query = query.Where(f => f.Nome.Contains(filter.Arguments.Nome));

            var dataSet = new PlanilhaFuncionario();
            dataSet.PageNumber = (filter.PageNumber > 0) ? filter.PageNumber : 1;
            dataSet.PageSize = (filter.PageSize < 5) ? 5 : ((filter.PageSize > 100) ? 100 : filter.PageSize);
            dataSet.RowCount = await query.CountAsync();
            dataSet.PageCount = (int)Math.Ceiling(dataSet.RowCount / (float)dataSet.PageSize);

            if (dataSet.RowCount == 0)
                return (HttpStatusCode.NotFound, dataSet);
            if (dataSet.PageNumber > dataSet.PageCount)
                return (HttpStatusCode.NoContent, dataSet);

            var columnSelector = new Dictionary<ColunaFuncionario, Expression<Func<Funcionario, object>>>
            {
                { ColunaFuncionario.Id, x => x.Id },
                { ColunaFuncionario.Matricula, x => x.Matricula },
                { ColunaFuncionario.Cpf, x => x.Cpf },
                { ColunaFuncionario.Nome, x => x.Nome },
                { ColunaFuncionario.DataVinculo, x => x.DataVinculo }
            };
            
            query = (filter.Order == SortingOrder.Desc) ? query.OrderByDescending(columnSelector[filter.OrderBy]) : query.OrderBy(columnSelector[filter.OrderBy]);

            dataSet.Results = await query
                .Skip((dataSet.PageNumber - 1) * dataSet.PageSize)
                .Take(dataSet.PageSize)
                .ToListAsync();

            return (HttpStatusCode.OK, dataSet);
        }
        
        public async Task<(HttpStatusCode, object?)> CreateOrUpdate(Funcionario record)
        {
            var foundId = await _context.Funcionarios
                .Where(X => X.Matricula == record.Matricula || X.Cpf == record.Cpf)
                .Select(X => X.Id)
                .FirstOrDefaultAsync();

            if (foundId != record.Id)
                return (HttpStatusCode.BadRequest, "Matrícula ou CPF em uso.");

            HttpStatusCode statusCode;

            if (record.Id > 0)
            {
                if (foundId != record.Id)
                    return (HttpStatusCode.NotFound, null);

                _context.Funcionarios.Update(record);
                statusCode = HttpStatusCode.OK;
            }
            else
            {
                _context.Funcionarios.Add(record);
                statusCode = HttpStatusCode.Created;
            }

            await _context.SaveChangesAsync();
            return (statusCode, record);
        }

        public async Task<(HttpStatusCode, object?)> Delete(int id)
        {
            var record = await _context.Funcionarios.FindAsync(id);

            if (record == null)
                return (HttpStatusCode.NotFound, null);

            _context.Funcionarios.Remove(record);
            await _context.SaveChangesAsync();

            return (HttpStatusCode.OK, null);
        }
    }
}
