using AutoMapper;
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
        Task<(HttpStatusCode, FuncionarioResult)> Read(FuncionarioQuery requestQuery);
        Task<(HttpStatusCode, object?)> CreateOrUpdate(FuncionarioForm requestForm);
        Task<(HttpStatusCode, object?)> Delete(int id);
    }

    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly GestaoDbContext _context;
        private readonly IMapper _mapper;

        public FuncionarioRepository(GestaoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(HttpStatusCode, Funcionario?)> Read(int id)
        {
            var entity = await _context.Funcionarios.FindAsync(id);
            if (entity == null)
                return (HttpStatusCode.NotFound, entity);
            return (HttpStatusCode.OK, entity);
        }

        public async Task<(HttpStatusCode, FuncionarioResult)> Read(FuncionarioQuery requestQuery)
        {
            var query = _context.Funcionarios.AsNoTracking();

            if (!string.IsNullOrEmpty(requestQuery.Filter?.Matricula))
                query = query.Where(x => x.Matricula == requestQuery.Filter.Matricula);
            else if (!string.IsNullOrEmpty(requestQuery.Filter?.Cpf))
                query = query.Where(x => x.Cpf == requestQuery.Filter.Cpf);
            else if (!string.IsNullOrEmpty(requestQuery.Filter?.Nome))
                query = query.Where(x => x.Nome.Contains(requestQuery.Filter.Nome));

            var result = new FuncionarioResult();
            result.PageNumber = (requestQuery.PageNumber > 0) ? requestQuery.PageNumber : 1;
            result.PageSize = (requestQuery.PageSize < 5) ? 5 : ((requestQuery.PageSize > 100) ? 100 : requestQuery.PageSize);
            result.RowCount = await query.CountAsync();
            result.PageCount = (int)Math.Ceiling(result.RowCount / (float)result.PageSize);

            if (result.RowCount == 0)
                return (HttpStatusCode.NotFound, result);
            if (result.PageNumber > result.PageCount)
                return (HttpStatusCode.NoContent, result);

            var columnSelector = new Dictionary<FuncionarioSortableColumn, Expression<Func<Funcionario, object>>>
            {
                { FuncionarioSortableColumn.Id, x => x.Id },
                { FuncionarioSortableColumn.Matricula, x => x.Matricula },
                { FuncionarioSortableColumn.Cpf, x => x.Cpf },
                { FuncionarioSortableColumn.Nome, x => x.Nome }
                //{ FuncionarioSortableColumn.DataVinculo, x => x.DataVinculo }
            };
            
            query = (requestQuery.Order == SortingOrder.Desc) ? query.OrderByDescending(columnSelector[requestQuery.OrderBy]) : query.OrderBy(columnSelector[requestQuery.OrderBy]);
            query = query
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize);

            result.Items = await _mapper.ProjectTo<FuncionarioRow>(query)
                .ToListAsync();

            return (HttpStatusCode.OK, result);
        }
        
        public async Task<(HttpStatusCode, object?)> CreateOrUpdate(FuncionarioForm requestForm)
        {
            var errors = new Dictionary<string, string[]>();

            var foundId = await _context.Funcionarios.AsNoTracking()
                .Where(x => x.Matricula == requestForm.Matricula || x.Cpf == requestForm.Cpf)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (foundId != requestForm.Id)
            {
                errors.Add(nameof(Funcionario), new[] { "Matrícula ou CPF em uso." });
                return (HttpStatusCode.BadRequest, errors);
            }

            HttpStatusCode statusCode;
            var entity = _mapper.Map<Funcionario>(requestForm);

            if (requestForm.Id > 0)
            {
                if (foundId != requestForm.Id)
                    return (HttpStatusCode.NotFound, null);

                _context.Funcionarios.Update(entity);
                statusCode = HttpStatusCode.OK;
            }
            else
            {
                _context.Funcionarios.Add(entity);
                statusCode = HttpStatusCode.Created;
            }

            await _context.SaveChangesAsync();
            return (statusCode, entity);
        }

        public async Task<(HttpStatusCode, object?)> Delete(int id)
        {
            var entity = await _context.Funcionarios.FindAsync(id);

            if (entity == null)
                return (HttpStatusCode.NotFound, null);

            _context.Funcionarios.Remove(entity);
            await _context.SaveChangesAsync();

            return (HttpStatusCode.OK, null);
        }
    }
}
