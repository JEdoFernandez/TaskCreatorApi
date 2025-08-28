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

        public async Task<List<Tarea>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Tarea>> GetPendientesByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioId == usuarioId && !t.Completada)
                .ToListAsync();
        }

        public async Task<List<Tarea>> GetCompletadasByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioId == usuarioId && t.Completada)
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

        public async Task<List<Tarea>> BuscarPorTituloAsync(int usuarioId, string titulo)
        {
            return await _context.Tareas
                .Where(t => t.UsuarioId == usuarioId && t.Titulo.Contains(titulo))
                .ToListAsync();
        }
    }
}