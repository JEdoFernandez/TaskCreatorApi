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
            
        
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                
            
            modelBuilder.Entity<TareaPublicaCompletada>()
                .HasKey(tpc => tpc.Id);

            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Tarea>().HasKey(t => t.Id);
            modelBuilder.Entity<TareaPublica>().HasKey(tp => tp.Id);
            modelBuilder.Entity<TareaPublicaCompletada>().HasKey(tpc => tpc.Id);
        }
    }
}