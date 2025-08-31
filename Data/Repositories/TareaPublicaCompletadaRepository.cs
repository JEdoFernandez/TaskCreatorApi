using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Data.Repositories
{
    public class TareaPublicaCompletadaRepository
    {
        private readonly AppDbContext _context;

        public TareaPublicaCompletadaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TareaPublicaCompletada>> GetAllAsync()
        {
            return await _context.TareasPublicasCompletadas.ToListAsync();
        }

        public async Task<List<TareaPublicaCompletada>> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.TareasPublicasCompletadas
                .Where(t => t.UsuarioNombre == usuarioNombre)
                .ToListAsync();
        }

        public async Task<TareaPublicaCompletada> AddAsync(TareaPublicaCompletada tareaCompletada)
        {
            _context.TareasPublicasCompletadas.Add(tareaCompletada);
            await _context.SaveChangesAsync();
            return tareaCompletada;
        }

        public async Task<bool> ExisteCompletadaAsync(int tareaPublicaId, string usuarioNombre)
        {
            return await _context.TareasPublicasCompletadas
                .AnyAsync(t => t.TareaPublicaId == tareaPublicaId && t.UsuarioNombre == usuarioNombre);
        }

        public async Task<TareaPublicaCompletada> GetByUsuarioYTareaAsync(int tareaPublicaId, string usuarioNombre)
        {
            return await _context.TareasPublicasCompletadas
                .FirstOrDefaultAsync(t => t.TareaPublicaId == tareaPublicaId && t.UsuarioNombre == usuarioNombre);
        }

        public async Task<List<TareaPublicaCompletada>> GetByTareaIdAsync(int tareaPublicaId)
        {
            return await _context.TareasPublicasCompletadas
                .Where(t => t.TareaPublicaId == tareaPublicaId)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var completada = await _context.TareasPublicasCompletadas.FindAsync(id);
            if (completada == null) return false;

            _context.TareasPublicasCompletadas.Remove(completada);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}