using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using backend.Helpers;

namespace backend.Services
{
    public interface IFeriasRepository
    {
        Task<FeriasGroupByExercicio?> Read(int funcionarioId, int exercicioId);
        Task<FeriasGroupByFuncionario?> Read(FeriasFilter requestFilter);
        Task<byte[]> Download(int id, string? sheetName = null);
        Task<FeriasGroupByExercicio?> Delete(int funcionarioId, int exercicioId);
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

        public async Task<FeriasGroupByExercicio?> Read(int funcionarioId, int exercicioId)
        {
            var entity = await _context.Exercicios.AsNoTracking()
                .Include(x => x.Ferias)
                .FirstOrDefaultAsync(x => x.Id == exercicioId && x.FuncionarioId == funcionarioId);

            if (entity == null)
                return null;

            return _mapper.Map<FeriasGroupByExercicio>(entity);
        }
        /*
        public async Task<FeriasGroupByFuncionario?> Read(FeriasFilter requestFilter)
        {
            var query = _context.Ferias.AsNoTracking();

            if (requestFilter?.Id > 0)
                query = query.Where(x => x.Exercicio.Funcionario.Id == requestFilter.Id);
            else if (!string.IsNullOrEmpty(requestFilter?.Cpf))
                query = query.Where(x => x.Exercicio.Funcionario.Cpf == requestFilter.Cpf);
            else if(!string.IsNullOrEmpty(requestFilter?.Matricula))
                query = query.Where(x => x.Exercicio.Funcionario.Matricula == requestFilter.Matricula);
            
            query = query
                .OrderBy(x => x.Exercicio.DataInicio).ThenBy(x => x.DataInicio);

            return await _mapper.ProjectTo<FeriasGroupByFuncionario>(
                    query.Select(x => x.Exercicio.Funcionario).Distinct()
                ).FirstOrDefaultAsync();
        }
        */
        public async Task<FeriasGroupByFuncionario?> Read(FeriasFilter requestFilter)
        {
            var query = _context.Funcionarios.AsNoTracking();

            if (requestFilter?.Id > 0)
                query = query.Where(x => x.Id == requestFilter.Id);
            else if (!string.IsNullOrEmpty(requestFilter?.Cpf))
                query = query.Where(x => x.Cpf == requestFilter.Cpf);
            else if(!string.IsNullOrEmpty(requestFilter?.Matricula))
                query = query.Where(x => x.Matricula == requestFilter.Matricula);

            query = query
                .Include(a => a.Exercicios!.OrderBy(b => b.DataInicio))
                .ThenInclude(b => b.Ferias!.OrderBy(c => c.DataInicio));

            return await _mapper.ProjectTo<FeriasGroupByFuncionario>(query)
                .FirstOrDefaultAsync();
        }

        public async Task<byte[]> Download(int id, string? sheetName = null)
        {
            return await Export.ToSpreadsheet(
                _mapper.Map<List<FeriasSpreadsheetRow>>(await Read(new() { Id = id })),
                sheetName ?? nameof(Ferias));
        }

        public async Task<FeriasGroupByExercicio?> Delete(int funcionarioId, int exercicioId)
        {
            var entity = await _context.Exercicios
                .Include(x => x.Ferias)
                .FirstOrDefaultAsync(x => x.Id == exercicioId && x.FuncionarioId == funcionarioId);

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

            _context.Ferias.RemoveRange(_context.Ferias.Where(x => x.ExercicioId == requestForm.Id));// remover férias do exercício, para inserir novas durante atualização
            _context.Exercicios.Update(entity);
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
