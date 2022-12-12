using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace backend.Services
{
    public class GestaoDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Ferias> Ferias { get; set; }

        public GestaoDbContext(IConfiguration configuration)//, IWebHostEnvironment environment)
        {
            //var match = Regex.Match(configuration.GetConnectionString("GestaoDb")!, @"(.+?=)(.+?\.db)(.*)");
            //var filePath = Path.Join(environment.ContentRootPath, "Data", Path.GetFileName(match.Groups[2].Value));
            //_connectionString = match.Result($"$1\"{filePath}\"$3");
            _connectionString = configuration.GetConnectionString("GestaoDb")!;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Usuario>().Property(x => x.Perfil).HasConversion<string>();
            //modelBuilder.Entity<Papel>().Property(e => e.Permissoes).HasConversion(new EnumCollectionConverter<Permissao>());

            //modelBuilder.Entity<Usuario>().HasData(new Usuario { Id = 1, Nome = "Rosangela Barra", Email = "rosangela@gmail.com", Senha = "1234", Perfil = Perfil.Administrador });
        }
        
        public class EnumCollectionConverter<T> : ValueConverter<ICollection<T>, string> where T : Enum
        {
            public EnumCollectionConverter() : base(
                v => JsonSerializer.Serialize(v.Select(e => e.ToString()).ToList(), (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null).Select(e => (T)Enum.Parse(typeof(T), e)).ToList(),
                new CollectionComparer<T>()
            ) { }
        }

        public class CollectionComparer<T> : ValueComparer<ICollection<T>>
        {
            public CollectionComparer() : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (ICollection<T>)c.ToHashSet()
            ) { }
        }
        */
    }
}
