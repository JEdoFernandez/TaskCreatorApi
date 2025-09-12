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
            
            // Configurar claves primarias
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Tarea>().HasKey(t => t.Id);
            modelBuilder.Entity<TareaPublica>().HasKey(tp => tp.Id);
            modelBuilder.Entity<TareaPublicaCompletada>().HasKey(tpc => tpc.Id);

            // Configurar relaciones y restricciones para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Contraseña).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Rol).IsRequired().HasMaxLength(20);
                entity.Property(u => u.FechaRegistro).IsRequired();
                entity.Property(u => u.Activo).IsRequired();
            });

            // Configurar relaciones y restricciones para Tarea (Privadas)
            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Descripcion).HasColumnType("TEXT");
                entity.Property(t => t.FechaCreacion).IsRequired();
                entity.Property(t => t.Prioridad).IsRequired();
                entity.Property(t => t.Categoria).HasMaxLength(100);
                entity.Property(t => t.UsuarioNombre).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Completada).IsRequired();
                
                // ✅ ELIMINADO: Relación con Usuario para evitar ciclos JSON
                // entity.HasOne(t => t.Usuario)
                //       .WithMany(u => u.Tareas)
                //       .HasForeignKey(t => t.UsuarioId)
                //       .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar relaciones y restricciones para TareaPublica
            modelBuilder.Entity<TareaPublica>(entity =>
            {
                entity.Property(tp => tp.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(tp => tp.Descripcion).HasColumnType("TEXT");
                entity.Property(tp => tp.PublicadoPor).IsRequired().HasMaxLength(100);
                entity.Property(tp => tp.FechaPublicacion).IsRequired();
                entity.Property(tp => tp.Prioridad).IsRequired();
                entity.Property(tp => tp.Categoria).HasMaxLength(100);
                entity.Property(tp => tp.Completada).IsRequired();
                entity.Property(tp => tp.Recomendado).IsRequired();
                
                // ✅ ELIMINADO: Relación circular con Completadas
                // entity.HasMany(tp => tp.Completadas)
                //       .WithOne(tpc => tpc.TareaPublica)
                //       .HasForeignKey(tpc => tpc.TareaPublicaId)
                //       .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar relaciones y restricciones para TareaPublicaCompletada
            modelBuilder.Entity<TareaPublicaCompletada>(entity =>
            {
                entity.Property(tpc => tpc.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(tpc => tpc.Descripcion).HasColumnType("TEXT");
                entity.Property(tpc => tpc.UsuarioNombre).IsRequired().HasMaxLength(100);
                entity.Property(tpc => tpc.FechaCompletado).IsRequired();
                entity.Property(tpc => tpc.TareaPublicaId).IsRequired();
                
                // ✅ ELIMINADO: Relación circular con TareaPublica
                // entity.HasOne(tpc => tpc.TareaPublica)
                //       .WithMany(tp => tp.Completadas)
                //       .HasForeignKey(tpc => tpc.TareaPublicaId)
                //       .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar valores por defecto
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.Activo).HasDefaultValue(true);
                entity.Property(u => u.Rol).HasDefaultValue("User");
            });

            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.Property(t => t.Completada).HasDefaultValue(false);
                entity.Property(t => t.Prioridad).HasDefaultValue(3);
            });

            modelBuilder.Entity<TareaPublica>(entity =>
            {
                entity.Property(tp => tp.Completada).HasDefaultValue(false);
                entity.Property(tp => tp.Recomendado).HasDefaultValue(false);
                entity.Property(tp => tp.Prioridad).HasDefaultValue(3);
            });

            // Configurar índices para mejorar el rendimiento
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Nombre).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email);
            
            modelBuilder.Entity<Tarea>().HasIndex(t => t.UsuarioNombre);
            modelBuilder.Entity<Tarea>().HasIndex(t => t.Completada);
            modelBuilder.Entity<Tarea>().HasIndex(t => t.Prioridad);
            modelBuilder.Entity<Tarea>().HasIndex(t => t.Categoria);
            
            modelBuilder.Entity<TareaPublica>().HasIndex(tp => tp.PublicadoPor);
            modelBuilder.Entity<TareaPublica>().HasIndex(tp => tp.Completada);
            modelBuilder.Entity<TareaPublica>().HasIndex(tp => tp.Prioridad);
            modelBuilder.Entity<TareaPublica>().HasIndex(tp => tp.Categoria);
            
            modelBuilder.Entity<TareaPublicaCompletada>().HasIndex(tpc => tpc.UsuarioNombre);
            modelBuilder.Entity<TareaPublicaCompletada>().HasIndex(tpc => tpc.TareaPublicaId);
            modelBuilder.Entity<TareaPublicaCompletada>().HasIndex(tpc => tpc.FechaCompletado);
        }
    }
}