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

        public async Task<List<Tarea>> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _repository.GetByUsuarioNombreAsync(usuarioNombre);
        }

        public async Task<List<Tarea>> GetPendientesByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _repository.GetPendientesByUsuarioNombreAsync(usuarioNombre);
        }

        public async Task<List<Tarea>> GetCompletadasByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _repository.GetCompletadasByUsuarioNombreAsync(usuarioNombre);
        }

        public async Task<Tarea> GetByTituloAsync(string usuarioNombre, string titulo)
        {
            var tareas = await _repository.BuscarPorTituloAsync(usuarioNombre, titulo);
            return tareas.FirstOrDefault();
        }

        public async Task<Tarea> CreateAsync(Tarea tarea)
        {
            tarea.FechaCreacion = DateTime.Now;
            tarea.Completada = false;
            tarea.FechaCompletado = null;
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

        public async Task<List<Tarea>> BuscarPorTituloYPrioridadAsync(string usuarioNombre, string titulo = null, int? prioridad = null)
        {
            return await _repository.BuscarPorTituloYPrioridadAsync(usuarioNombre, titulo, prioridad);
        }
    }
}