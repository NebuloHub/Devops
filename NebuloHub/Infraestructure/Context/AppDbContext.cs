using Microsoft.EntityFrameworkCore;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Avaliacao> Avaliacao { get; set; }
        public DbSet<Habilidade> Habilidade { get; set; }
        public DbSet<Possui> Possui { get; set; }
        public DbSet<Startup> Startup { get; set; }
        public DbSet<Usuario> Usuario { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
