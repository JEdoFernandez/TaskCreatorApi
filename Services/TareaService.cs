using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Services
{
    public class TareaService
    {
        private readonly TareaRepository _repository;

        public TareaService(TareaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Tarea>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _repository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<List<Tarea>> GetPendientesByUsuarioIdAsync(int usuarioId)
        {
            return await _repository.GetPendientesByUsuarioIdAsync(usuarioId);
        }

        public async Task<List<Tarea>> GetCompletadasByUsuarioIdAsync(int usuarioId)
        {
            return await _repository.GetCompletadasByUsuarioIdAsync(usuarioId);
        }

        public async Task<Tarea> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Tarea> CreateAsync(Tarea tarea)
        {
            tarea.FechaCreacion = DateTime.Now;
            return await _repository.AddAsync(tarea);
        }

        public async Task<bool> UpdateAsync(Tarea tarea)
        {
            return await _repository.UpdateAsync(tarea);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> MarcarComoCompletadaAsync(int id)
        {
            var tarea = await _repository.GetByIdAsync(id);
            if (tarea == null) return false;

            tarea.Completada = true;
            tarea.FechaCompletado = DateTime.Now;
            return await _repository.UpdateAsync(tarea);
        }

        public async Task<List<Tarea>> BuscarPorTituloAsync(int usuarioId, string titulo)
        {
            return await _repository.BuscarPorTituloAsync(usuarioId, titulo);
        }
    }
}