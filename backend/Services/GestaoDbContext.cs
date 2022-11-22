using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class GestaoDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Exercicio> Exercicio { get; set; }
        public DbSet<Ferias> Ferias { get; set; }

        public GestaoDbContext(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var match = Regex.Match(configuration.GetConnectionString("GestaoDb")!, @"(.+?=)(.+?\.db)(.*)");
            var filePath = Path.Join(environment.ContentRootPath, "Data", Path.GetFileName(match.Groups[2].Value));
            _connectionString = match.Result($"$1\"{filePath}\"$3");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
