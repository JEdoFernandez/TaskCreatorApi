using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;
using TaskCreatorAPI.Models.DTOs;

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

        public async Task<Usuario> GetByNombreAsync(string nombre)
        {
            return await _repository.GetByNombreAsync(nombre);
        }

        public async Task<bool> UpdateByNombreAsync(string nombre, UsuarioUpdateDTO dto)
        {
            var usuarioExistente = await _repository.GetByNombreAsync(nombre);
            if (usuarioExistente == null) return false;

            // Actualizar solo las propiedades permitidas
            if (!string.IsNullOrEmpty(dto.Email))
                usuarioExistente.Email = dto.Email;
            
            if (!string.IsNullOrEmpty(dto.Contraseña))
                usuarioExistente.Contraseña = dto.Contraseña; // Sin hash
            
            usuarioExistente.Activo = dto.Activo;
            usuarioExistente.Rol = dto.Rol;

            return await _repository.UpdateAsync(usuarioExistente);
        }

        public async Task<bool> DeleteByNombreAsync(string nombre)
        {
            var usuario = await _repository.GetByNombreAsync(nombre);
            if (usuario == null) return false;

            return await _repository.DeleteAsync(usuario.Id);
        }
    }
}