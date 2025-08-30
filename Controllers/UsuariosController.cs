using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Models;
using TaskCreatorAPI.Models.DTOs;

namespace TaskCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuariosController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetAll()
        {
            var usuarios = await _service.GetAllAsync();
            return Ok(usuarios);
        }

        // 🔥 SOLO ENDPOINTS POR NOMBRE
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<Usuario>> GetByNombre(string nombre)
        {
            var usuario = await _service.GetByNombreAsync(nombre);
            if (usuario == null) return NotFound(new { mensaje = $"Usuario '{nombre}' no encontrado" });
            return Ok(usuario);
        }

        [HttpPut("nombre/{nombre}")]
        public async Task<IActionResult> UpdateByNombre(string nombre, [FromBody] UsuarioUpdateDTO dto)
        {
            var actualizado = await _service.UpdateByNombreAsync(nombre, dto);
            if (!actualizado) return NotFound(new { mensaje = $"Usuario '{nombre}' no encontrado" });
            return Ok(new { mensaje = $"Usuario '{nombre}' actualizado correctamente" });
        }

        [HttpDelete("nombre/{nombre}")]
        public async Task<IActionResult> DeleteByNombre(string nombre)
        {
            var eliminado = await _service.DeleteByNombreAsync(nombre);
            if (!eliminado) return NotFound(new { mensaje = $"Usuario '{nombre}' no encontrado" });
            return Ok(new { mensaje = $"Usuario '{nombre}' eliminado correctamente" });
        }

        // 🔥 ENDPOINT EXTRA: Buscar usuarios por coincidencia de nombre
        [HttpGet("buscar/{termino}")]
        public async Task<ActionResult<List<Usuario>>> BuscarUsuarios(string termino)
        {
            var todosUsuarios = await _service.GetAllAsync();
            var usuariosFiltrados = todosUsuarios
                .Where(u => u.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            return Ok(usuariosFiltrados);
        }
    }
}