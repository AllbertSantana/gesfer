using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace backend.Services
{
    public interface IFeriasRepository
    {
        Task<FeriasGroupByExercicio?> Read(int id);
        Task<FeriasResult> Read(FeriasQuery requestQuery, int maxSize = 100);
        Task<FeriasGroupByExercicio?> Delete(int id);
        Task<(FeriasGroupByExercicio?, Dictionary<string, string[]>)> Create(ExercicioForm requestForm);
        Task<(FeriasGroupByExercicio?, Dictionary<string, string[]>)> Update(ExercicioForm requestForm);
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

        public async Task<FeriasGroupByExercicio?> Read(int id)
        {
            var entity = await _context.Exercicios.AsNoTracking()
                .Include(x => x.Ferias)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            return _mapper.Map<FeriasGroupByExercicio>(entity);
        }

        public async Task<FeriasResult> Read(FeriasQuery requestQuery, int maxSize = 100)
        {
            var query = _context.Ferias.AsNoTracking();

            if (!string.IsNullOrEmpty(requestQuery.Filter?.Matricula))
                query = query.Where(x => x.Exercicio.Funcionario.Matricula == requestQuery.Filter.Matricula);
            else if (!string.IsNullOrEmpty(requestQuery.Filter?.Cpf))
                query = query.Where(x => x.Exercicio.Funcionario.Cpf == requestQuery.Filter.Cpf);

            var result = new FeriasResult();
            result.PageNumber = (requestQuery.PageNumber > 0) ? requestQuery.PageNumber : 1;
            result.PageSize = (requestQuery.PageSize < 5) ? 5 : ((requestQuery.PageSize > maxSize) ? maxSize : requestQuery.PageSize);
            result.RowCount = await query.CountAsync();
            result.PageCount = (int)Math.Ceiling(result.RowCount / (float)result.PageSize);

            if (result.RowCount == 0)
                return result;
            if (result.PageNumber > result.PageCount)
                result.PageNumber = result.PageCount;

            if (requestQuery.Order == SortingOrder.Desc)
                query = query.OrderByDescending(x => x.Exercicio.DataInicio).ThenByDescending(x => x.DataInicio);
            else
                query = query.OrderBy(x => x.Exercicio.DataInicio).ThenBy(x => x.DataInicio);

            query = query
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize)
                .Include(x => x.Exercicio);//.Include(x => x.Exercicio.Funcionario);

            result.Items = await _mapper.ProjectTo<FeriasGroupByExercicio>(
                    query.Select(x => x.Exercicio).Distinct()
                ).ToListAsync();
            
            return result;
        }

        public async Task<FeriasGroupByExercicio?> Delete(int id)
        {
            var entity = await _context.Exercicios
                .Include(x => x.Ferias)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            _context.Exercicios.Remove(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<FeriasGroupByExercicio>(entity);
        }

        public async Task<(FeriasGroupByExercicio?, Dictionary<string, string[]>)> Create(ExercicioForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Exercicio>(requestForm);

            _context.Exercicios.Add(entity);
            await _context.SaveChangesAsync();
            /*try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException && ((SqliteException)ex.InnerException).SqliteErrorCode == 19 && !(await _context.Funcionarios.AsNoTracking().AnyAsync(x => x.Id == requestForm.FuncionarioId)))
                    return (null, errors);
                throw;
            }*/
            return (_mapper.Map<FeriasGroupByExercicio>(entity), errors);
        }

        public async Task<(FeriasGroupByExercicio?, Dictionary<string, string[]>)> Update(ExercicioForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Exercicio>(requestForm);

            _context.Exercicios.Update(entity);// deleta e insere novas férias, ao invés de atualizá-las
            await _context.SaveChangesAsync();
            /*try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await _context.Exercicios.AsNoTracking().AnyAsync(x => x.Id == requestForm.Id)))
                    return (null, errors);
                throw;
            }*/
            return (_mapper.Map<FeriasGroupByExercicio>(entity), errors);
        }

        private async Task<Dictionary<string, string[]>> Validate(ExercicioForm requestForm)
        {
            var errors = new Dictionary<string, string[]>();

            var exists = false;
            if (requestForm.Id > 0)// update
            {
                exists = await _context.Exercicios.AsNoTracking()
                    .AnyAsync(x => x.Id == requestForm.Id && x.FuncionarioId == requestForm.FuncionarioId);
                if (!exists)
                    errors.Add(nameof(Exercicio), new[] { "Período aquisitivo não existe" });
            }
            else// create
            {
                exists = await _context.Funcionarios.AsNoTracking()
                    .AnyAsync(x => x.Id == requestForm.FuncionarioId);
                if (!exists)
                    errors.Add(nameof(Funcionario), new[] { "Funcionário não existe" });
            }
            if (!exists)
                return errors;

            var overlaps = await _context.Exercicios.AsNoTracking()
                .Where(x => x.FuncionarioId == requestForm.FuncionarioId && x.Id != requestForm.Id)
                .AnyAsync(x => requestForm.DataInicio <= x.DataFim && requestForm.DataFim >= x.DataInicio);

            if (overlaps)
                errors.Add(nameof(Exercicio), new[] { "Período aquisitivo não pode se sobrepor a outro" });

            if (requestForm.Ferias != null)
            {
                var i = 0;
                foreach (var ferias in requestForm.Ferias)
                {
                    overlaps = await _context.Ferias.AsNoTracking()
                        .Where(x => x.Exercicio.FuncionarioId == requestForm.FuncionarioId && x.ExercicioId != requestForm.Id)
                        .AnyAsync(x => ferias.DataInicio <= x.DataFim && ferias.DataFim >= x.DataInicio);

                    if (overlaps)
                        errors.Add($"{nameof(Ferias)}[{i}]", new[] { "Período de férias não pode se sobrepor a outro" });
                    i++;
                }
            }

            return errors;
        }
    }
}
