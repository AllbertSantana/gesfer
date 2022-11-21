using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace backend.Services
{
    public class GestaoDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Exercicio> Exercicio { get; set; }
        public DbSet<Ferias> Ferias { get; set; }

        public GestaoDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("GestaoDb")!;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
