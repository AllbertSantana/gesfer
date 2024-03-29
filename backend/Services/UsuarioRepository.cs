﻿using AutoMapper;
using backend.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backend.Services
{
    public interface IUsuarioRepository
    {
        Task<UsuarioAccount?> Read(int id);
        Task<UsuarioResult> Read(UsuarioQuery requestQuery, int maxSize = 100);
        Task<UsuarioAccount?> Delete(int id);
        Task<(UsuarioAccount?, Dictionary<string, string[]>)> Create(SignupForm requestForm);
        Task<(UsuarioAccount?, Dictionary<string, string[]>)> Update(SignupForm requestForm);
        Task<(UsuarioAccount?, string?)> Authenticate(LoginForm requestForm);
        string GenerateToken(IEnumerable<Claim> claims);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly GestaoDbContext _context;
        private readonly IMapper _mapper;
        private readonly byte[] _jwtKey;

        public UsuarioRepository(GestaoDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _jwtKey = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
        }

        public async Task<UsuarioAccount?> Read(int id)
        {
            var entity = await _context.Usuarios.FindAsync(id);
            if (entity == null)
                return null;
            return _mapper.Map<UsuarioAccount>(entity);
        }

        public async Task<UsuarioResult> Read(UsuarioQuery requestQuery, int maxSize = 100)
        {
            var query = _context.Usuarios.AsNoTracking();
            
            if (!string.IsNullOrWhiteSpace(requestQuery.Filter?.Cpf))
                query = query.Where(x => x.Cpf == requestQuery.Filter.Cpf);
            else if (!string.IsNullOrWhiteSpace(requestQuery.Filter?.Email))
                query = query.Where(x => x.Email.Contains(requestQuery.Filter.Email));
            else if (!string.IsNullOrWhiteSpace(requestQuery.Filter?.Nome))
                query = query.Where(x => x.Nome.Contains(requestQuery.Filter.Nome));
            
            if (requestQuery.Filter?.Perfil > 0)
                query = query.Where(x => x.Perfil == requestQuery.Filter.Perfil);

            var result = new UsuarioResult();
            result.PageNumber = (requestQuery.PageNumber > 0) ? requestQuery.PageNumber : 1;
            result.PageSize = (requestQuery.PageSize < 5) ? 5 : ((requestQuery.PageSize > maxSize) ? maxSize : requestQuery.PageSize);
            result.RowCount = await query.CountAsync();
            result.PageCount = (int)Math.Ceiling(result.RowCount / (float)result.PageSize);

            if (result.RowCount == 0)
                return result;
            if (result.PageNumber > result.PageCount)
                result.PageNumber = result.PageCount;

            var columnSelector = new Dictionary<UsuarioSortableColumn, Expression<Func<Usuario, object>>>
            {
                { UsuarioSortableColumn.Id, x => x.Id },
                { UsuarioSortableColumn.Cpf, x => x.Cpf },
                { UsuarioSortableColumn.Nome, x => x.Nome },
                { UsuarioSortableColumn.Email, x => x.Email },
                { UsuarioSortableColumn.Perfil, x => x.Perfil }
            };

            query = (requestQuery.Order == SortingOrder.Desc) ? query.OrderByDescending(columnSelector[requestQuery.OrderBy]) : query.OrderBy(columnSelector[requestQuery.OrderBy]);
            query = query
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize);

            result.Items = await _mapper.ProjectTo<UsuarioSummary>(query)
                .ToListAsync();

            return result;
        }

        public async Task<UsuarioAccount?> Delete(int id)
        {
            var entity = await _context.Usuarios.FindAsync(id);
            if (entity == null)
                return null;

            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<UsuarioAccount>(entity);
        }

        public async Task<(UsuarioAccount?, Dictionary<string, string[]>)> Create(SignupForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Usuario>(requestForm);
            entity.Senha = HashPassword(requestForm.Senha);

            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();
            return (_mapper.Map<UsuarioAccount>(entity), errors);
        }

        public async Task<(UsuarioAccount?, Dictionary<string, string[]>)> Update(SignupForm requestForm)
        {
            var errors = await Validate(requestForm);
            if (errors.Count > 0)
                return (null, errors);

            var entity = _mapper.Map<Usuario>(requestForm);
            _context.Usuarios.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;//_context.Usuarios.Update(entity);

            if (requestForm.Perfil == default)
                _context.Entry(entity).Property(x => x.Perfil).IsModified = false;

            if (string.IsNullOrWhiteSpace(requestForm.Senha))
                _context.Entry(entity).Property(x => x.Senha).IsModified = false;
            else
                entity.Senha = HashPassword(requestForm.Senha);

            //foreach (var p in _context.Entry(entity).Properties)
            //    Console.WriteLine($"{p.Metadata.Name} = \"{p.CurrentValue}\", IsModified = \"{p.IsModified}\".");

            await _context.SaveChangesAsync();
            await _context.Entry(entity).ReloadAsync();
            return (_mapper.Map<UsuarioAccount>(entity), errors);
        }

        private async Task<Dictionary<string, string[]>> Validate(SignupForm requestForm)
        {
            var errors = new Dictionary<string, string[]>();

            var exists = false;
            if (requestForm.Id > 0)// update
            {
                exists = await _context.Usuarios.AsNoTracking()
                    .AnyAsync(x => x.Id == requestForm.Id);
                if (!exists)
                {
                    errors.Add(nameof(Usuario), new[] { "Usuário não existe" });
                    return errors;
                }
            }

            exists = await _context.Usuarios.AsNoTracking()
                .AnyAsync(x => x.Id != requestForm.Id && (x.Email == requestForm.Email || x.Cpf == requestForm.Cpf));

            if (exists)
                errors.Add(nameof(Usuario), new[] { "E-mail ou CPF pertence a outro usuário" });

            return errors;
        }

        public async Task<(UsuarioAccount?, string?)> Authenticate(LoginForm requestForm)
        {
            Usuario? entity = null;

            if (!string.IsNullOrWhiteSpace(requestForm.Email))
                entity = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == requestForm.Email);
            else if (!string.IsNullOrWhiteSpace(requestForm.Cpf))
                entity = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cpf == requestForm.Cpf);

            if (entity == null || !VerifyPassword(requestForm.Senha, entity.Senha))
                return (null, null);

            var token = GenerateToken(new[]
            {
                new Claim(nameof(Usuario.Id), entity.Id.ToString()),
                new Claim(nameof(Usuario.Perfil), entity.Perfil.ToString())
            });

            return (_mapper.Map<UsuarioAccount>(entity), token);
        }

        private static string HashPassword(string password, byte[]? salt = null)
        {
            if (salt == null)
                salt = RandomNumberGenerator.GetBytes(128 / 8);
            
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{hashed}:{Convert.ToBase64String(salt)}";// hashed password with salt
        }

        private static bool VerifyPassword(string password, string hashedPasswordWithSalt)
        {
            var salt = Convert.FromBase64String(hashedPasswordWithSalt.Split(':').Last());
            var hashed = HashPassword(password, salt).Split(':').First();// given password hash
            
            return hashedPasswordWithSalt.StartsWith($"{hashed}:");// compare both hashes
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
