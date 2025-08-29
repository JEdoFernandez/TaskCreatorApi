using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Forzar la creación de la base de datos al instanciar el contexto
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EnsureCreated: {ex.Message}");
            }
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<TareaPublica> TareasPublicas { get; set; }
        public DbSet<TareaPublicaCompletada> TareasPublicasCompletadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 🔥 NOMBRES EXPLÍCITOS DE TABLAS - Esto es crucial
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Tarea>().ToTable("Tareas");
            modelBuilder.Entity<TareaPublica>().ToTable("TareasPublicas");
            modelBuilder.Entity<TareaPublicaCompletada>().ToTable("TareasPublicasCompletadas");
            
            // Configurar relaciones
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<TareaPublicaCompletada>()
                .HasOne(tpc => tpc.TareaPublica)
                .WithMany(tp => tp.Completadas)
                .HasForeignKey(tpc => tpc.TareaPublicaId)
                .OnDelete(DeleteBehavior.Cascade);

            Console.WriteLine("✅ Modelo de base de datos configurado");
        }
    }
}