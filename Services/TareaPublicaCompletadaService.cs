using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Services
{
    public class TareaPublicaCompletadaService
    {
        private readonly TareaPublicaCompletadaRepository _repository;

        public TareaPublicaCompletadaService(TareaPublicaCompletadaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TareaPublicaCompletada>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<TareaPublicaCompletada>> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            return await _repository.GetByUsuarioNombreAsync(usuarioNombre);
        }

        public async Task<bool> ExisteCompletadaAsync(int tareaPublicaId, string usuarioNombre)
        {
            return await _repository.ExisteCompletadaAsync(tareaPublicaId, usuarioNombre);
        }
    }
}