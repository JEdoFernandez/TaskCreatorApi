using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Models;
using TaskCreatorAPI.Models.DTOs;
using System.Security.Claims;

namespace TaskCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Admin")]
    public class TareasPrivadasController : ControllerBase
    {
        private readonly TareaService _service;

        public TareasPrivadasController(TareaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tarea>>> Get()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tareas = await _service.GetByUsuarioIdAsync(usuarioId);
            return Ok(tareas);
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<List<Tarea>>> GetPendientes()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tareas = await _service.GetPendientesByUsuarioIdAsync(usuarioId);
            return Ok(tareas);
        }

        [HttpGet("completadas")]
        public async Task<ActionResult<List<Tarea>>> GetCompletadas()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tareas = await _service.GetCompletadasByUsuarioIdAsync(usuarioId);
            return Ok(tareas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetById(int id)
        {
            var tarea = await _service.GetByIdAsync(id);
            if (tarea == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (tarea.UsuarioId != usuarioId) return Forbid();

            return Ok(tarea);
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Create([FromBody] TareaCreateDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaLimite = dto.FechaLimite,
                Prioridad = dto.Prioridad,
                Categoria = dto.Categoria,
                UsuarioId = usuarioId,
                Completada = false
            };

            var creada = await _service.CreateAsync(tarea);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TareaCreateDTO dto)
        {
            var tarea = await _service.GetByIdAsync(id);
            if (tarea == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (tarea.UsuarioId != usuarioId) return Forbid();

            tarea.Titulo = dto.Titulo;
            tarea.Descripcion = dto.Descripcion;
            tarea.FechaLimite = dto.FechaLimite;
            tarea.Prioridad = dto.Prioridad;
            tarea.Categoria = dto.Categoria;

            await _service.UpdateAsync(tarea);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _service.GetByIdAsync(id);
            if (tarea == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (tarea.UsuarioId != usuarioId) return Forbid();

            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/completar")]
        public async Task<IActionResult> Completar(int id)
        {
            var tarea = await _service.GetByIdAsync(id);
            if (tarea == null) return NotFound();

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (tarea.UsuarioId != usuarioId) return Forbid();

            await _service.MarcarComoCompletadaAsync(id);
            return NoContent();
        }

        [HttpGet("buscar/{titulo}")]
        public async Task<ActionResult<List<Tarea>>> Buscar(string titulo)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tareas = await _service.BuscarPorTituloAsync(usuarioId, titulo);
            return Ok(tareas);
        }
    }
}