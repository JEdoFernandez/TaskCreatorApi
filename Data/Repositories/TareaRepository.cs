using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Data.Repositories
{
    public class TareaRepository
    {
        private readonly AppDbContext _context;

        public TareaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tarea>> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioNombre == usuarioNombre)
                .ToListAsync();
        }

        public async Task<List<Tarea>> GetPendientesByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioNombre == usuarioNombre && !t.Completada)
                .ToListAsync();
        }

        public async Task<List<Tarea>> GetCompletadasByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioNombre == usuarioNombre && t.Completada)
                .ToListAsync();
        }

        public async Task<Tarea> GetByIdAsync(int id)
        {
            return await _context.Tareas.FindAsync(id);
        }

        public async Task<Tarea> AddAsync(Tarea tarea)
        {
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();
            return tarea;
        }

        public async Task<bool> UpdateAsync(Tarea tarea)
        {
            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return false;

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Tarea>> BuscarPorTituloAsync(string usuarioNombre, string titulo)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioNombre == usuarioNombre && t.Titulo.Contains(titulo))
                .ToListAsync();
        }

        public async Task<List<Tarea>> BuscarPorTituloYPrioridadAsync(string usuarioNombre, string titulo = null, int? prioridad = null)
        {
            var query = _context.Tareas.Where(t => t.UsuarioNombre == usuarioNombre);

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(t => t.Titulo.Contains(titulo));
            }

            if (prioridad.HasValue)
            {
                query = query.Where(t => t.Prioridad == prioridad.Value);
            }

            return await query.ToListAsync();
        }
    }
}