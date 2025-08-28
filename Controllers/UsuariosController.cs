using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Models;

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
        public async Task<ActionResult<List<Usuario>>> Get()
        {
            var usuarios = await _service.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            var usuario = await _service.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _service.UpdateAsync(usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _service.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}