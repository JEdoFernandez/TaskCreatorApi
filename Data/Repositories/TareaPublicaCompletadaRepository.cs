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
            return await _context.TareasPublicasCompletadas
                .Include(t => t.TareaPublica)
                .ToListAsync();
        }

        public async Task<List<TareaPublicaCompletada>> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _context.TareasPublicasCompletadas
                .Where(t => t.UsuarioNombre == usuarioNombre)
                .Include(t => t.TareaPublica)
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
    }
}