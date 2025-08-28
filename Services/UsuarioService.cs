using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repository;

        public UsuarioService(UsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Usuario> GetByNombreAsync(string nombre)
        {
            return await _repository.GetByNombreAsync(nombre);
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            return await _repository.UpdateAsync(usuario);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}