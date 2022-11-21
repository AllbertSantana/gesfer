using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace backend.Services
{
    public class GestaoDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Ferias> Ferias { get; set; }

        public GestaoDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_configuration.GetConnectionString("GestaoDb"));
        }
    }
}
