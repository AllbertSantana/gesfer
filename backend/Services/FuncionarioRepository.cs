using AutoMapper;
using backend.Models;
using backend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Drawing.Printing;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.Services
{
    public interface IFuncionarioRepository
    {
        Task<FuncionarioRow?> Read(int id);
        Task<FuncionarioResult> Read(FuncionarioQuery requestQuery, int maxSize = 100);
        Task<QueryFileResult> Download(FuncionarioQuery requestQuery, string? sheetName = null);
        Task<FuncionarioRow?> Delete(int id);
        Task<IEnumerable<FuncionarioRow>> Delete(int[] batch);
        Task<(FuncionarioRow?, Dictionary<string, string[]>)> Create(FuncionarioForm requestForm);
        Task<(FuncionarioRow?, Dictionary<string, string[]>)> Update(FuncionarioForm requestForm);
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

        public async Task<FuncionarioRow?> Read(int id)
        {
            var entity = await _context.Funcionarios.FindAsync(id);
            if (entity == null)
                return null;
            return _mapper.Map<FuncionarioRow>(entity);
        }

        public async Task<FuncionarioResult> Read(FuncionarioQuery requestQuery, int maxSize = 100)
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
            result.PageSize = (requestQuery.PageSize < 5) ? 5 : ((requestQuery.PageSize > maxSize) ? maxSize : requestQuery.PageSize);
            result.RowCount = await query.CountAsync();
            result.PageCount = (int)Math.Ceiling(result.RowCount / (float)result.PageSize);

            if (result.RowCount == 0)
                return result;
            if (result.PageNumber > result.PageCount)
                result.PageNumber = result.PageCount;

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

            return result;
        }

        public async Task<QueryFileResult> Download(FuncionarioQuery requestQuery, string? sheetName = null)
        {
            var result = await Read(requestQuery, 100000);

            return new QueryFileResult
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                PageCount = result.PageCount,
                RowCount = result.RowCount,
                Contents = await Export.ToSpreadsheet(
                    _mapper.Map<List<FuncionarioSpreadsheetRow>>(result.Items),
                    sheetName ?? nameof(Funcionario))
            };
        }

        public async Task<FuncionarioRow?> Delete(int id)
        {
            var entity = await _context.Funcionarios.FindAsync(id);
            if (entity == null)
                return null;

            _context.Funcionarios.Remove(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<FuncionarioRow>(entity);
        }

        public async Task<IEnumerable<FuncionarioRow>> Delete(int[] batch)
        {
            var affected = await _context.Funcionarios.Where(x => batch.Contains(x.Id)).ToListAsync();
            _context.Funcionarios.RemoveRange(affected);
            await _context.SaveChangesAsync();
            return _mapper.Map<List<FuncionarioRow>>(affected);
        }

        public async Task<(FuncionarioRow?, Dictionary<string, string[]>)> Create(FuncionarioForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Funcionario>(requestForm);

            _context.Funcionarios.Add(entity);
            await _context.SaveChangesAsync();
            return (_mapper.Map<FuncionarioRow>(entity), errors);
        }

        public async Task<(FuncionarioRow?, Dictionary<string, string[]>)> Update(FuncionarioForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Funcionario>(requestForm);

            _context.Funcionarios.Update(entity);
            await _context.SaveChangesAsync();
            /*try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await _context.Funcionarios.AsNoTracking().AnyAsync(x => x.Id == requestForm.Id)))
                    return (null, errors);
                throw;
            }*/
            return (_mapper.Map<FuncionarioRow>(entity), errors);
        }

        private async Task<Dictionary<string, string[]>> Validate(FuncionarioForm requestForm)
        {
            var errors = new Dictionary<string, string[]>();

            var exists = false;
            if (requestForm.Id > 0)// update
            {
                exists = await _context.Funcionarios.AsNoTracking()
                    .AnyAsync(x => x.Id == requestForm.Id);
                if (!exists)
                {
                    errors.Add(nameof(Funcionario), new[] { "Funcionário não existe" });
                    return errors;
                }
            }

            exists = await _context.Funcionarios.AsNoTracking()
                .AnyAsync(x => x.Id != requestForm.Id && (x.Matricula == requestForm.Matricula || x.Cpf == requestForm.Cpf));

            if (exists)
                errors.Add(nameof(Funcionario), new[] { "Matrícula ou CPF pertence a outro funcionário" });

            return errors;
        }
    }
}
