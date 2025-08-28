using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<TareaPublica> TareasPublicas { get; set; }
        public DbSet<TareaPublicaCompletada> TareasPublicasCompletadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurar relaciones
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId);
                
            modelBuilder.Entity<TareaPublicaCompletada>()
                .HasOne(tpc => tpc.TareaPublica)
                .WithMany()
                .HasForeignKey(tpc => tpc.TareaPublicaId);
        }
    }
}