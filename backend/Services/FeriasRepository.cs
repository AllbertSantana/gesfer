using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace backend.Services
{
    public interface IFeriasRepository
    {
        Task<(HttpStatusCode, FeriasResult)> Read(FeriasQuery filter);
    }

    public class FeriasRepository : IFeriasRepository
    {
        private readonly GestaoDbContext _context;
        private readonly IMapper _mapper;

        public FeriasRepository(GestaoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(HttpStatusCode, FeriasResult)> Read(FeriasQuery requestQuery)
        {
            var query = _context.Ferias.AsNoTracking();

            if (!string.IsNullOrEmpty(requestQuery.Filter?.Matricula))
                query = query.Where(x => x.Exercicio.Funcionario.Matricula == requestQuery.Filter.Matricula);
            else if (!string.IsNullOrEmpty(requestQuery.Filter?.Cpf))
                query = query.Where(x => x.Exercicio.Funcionario.Cpf == requestQuery.Filter.Cpf);

            var result = new FeriasResult();
            result.PageNumber = (requestQuery.PageNumber > 0) ? requestQuery.PageNumber : 1;
            result.PageSize = (requestQuery.PageSize < 5) ? 5 : ((requestQuery.PageSize > 100) ? 100 : requestQuery.PageSize);
            result.RowCount = await query.CountAsync();
            result.PageCount = (int)Math.Ceiling(result.RowCount / (float)result.PageSize);

            if (result.RowCount == 0)
                return (HttpStatusCode.NotFound, result);
            if (result.PageNumber > result.PageCount)
                return (HttpStatusCode.NoContent, result);

            if (requestQuery.Order == SortingOrder.Desc)
                query = query.OrderByDescending(x => x.Exercicio.DataInicio).ThenByDescending(x => x.DataInicio);
            else
                query = query.OrderBy(x => x.Exercicio.DataInicio).ThenBy(x => x.DataInicio);

            query = query
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize)
                .Include(x => x.Exercicio);
                //.Include(x => x.Exercicio.Funcionario);

            result.Items = await _mapper.ProjectTo<FeriasGroupByExercicio>(query)
                .ToListAsync();
            
            return (HttpStatusCode.OK, result);
        }
    }
}
