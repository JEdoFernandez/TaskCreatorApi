using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Data.Repositories
{
    public class TareaPublicaRepository
    {
        private readonly AppDbContext _context;

        public TareaPublicaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TareaPublica>> GetAllAsync()
        {
            return await _context.TareasPublicas.ToListAsync();
        }

        public async Task<TareaPublica> GetByIdAsync(int id)
        {
            return await _context.TareasPublicas.FindAsync(id);
        }

        public async Task<TareaPublica> AddAsync(TareaPublica tareaPublica)
        {
            _context.TareasPublicas.Add(tareaPublica);
            await _context.SaveChangesAsync();
            return tareaPublica;
        }

        public async Task<bool> UpdateAsync(TareaPublica tareaPublica)
        {
            _context.TareasPublicas.Update(tareaPublica);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tareaPublica = await _context.TareasPublicas.FindAsync(id);
            if (tareaPublica == null) return false;

            _context.TareasPublicas.Remove(tareaPublica);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TareaPublica>> BuscarPorTituloAsync(string titulo)
        {
            return await _context.TareasPublicas
                .Where(t => t.Titulo.Contains(titulo))
                .ToListAsync();
        }

        public async Task IncrementarVecesCompletadaAsync(int id)
        {
            var tareaPublica = await _context.TareasPublicas.FindAsync(id);
            if (tareaPublica != null)
            {
                tareaPublica.VecesCompletada++;
                await _context.SaveChangesAsync();
            }
        }
    }
}