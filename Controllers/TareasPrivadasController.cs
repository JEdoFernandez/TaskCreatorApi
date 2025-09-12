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
        public async Task<ActionResult<List<object>>> Get()
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareas = await _service.GetByUsuarioNombreAsync(usuarioNombre);
            
            var tareasFormateadas = tareas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                FechaCreacion = t.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                t.Completada,
                t.Prioridad,
                t.Categoria,
                t.UsuarioNombre
            }).ToList();
            
            return Ok(tareasFormateadas);
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<List<object>>> GetPendientes()
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareas = await _service.GetPendientesByUsuarioNombreAsync(usuarioNombre);
            
            var tareasFormateadas = tareas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                FechaCreacion = t.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                t.Completada,
                t.Prioridad,
                t.Categoria,
                t.UsuarioNombre
            }).ToList();
            
            return Ok(tareasFormateadas);
        }

        [HttpGet("completadas")]
        public async Task<ActionResult<List<object>>> GetCompletadas()
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareas = await _service.GetCompletadasByUsuarioNombreAsync(usuarioNombre);
            
            var tareasFormateadas = tareas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                FechaCreacion = t.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                t.Completada,
                t.Prioridad,
                t.Categoria,
                t.UsuarioNombre
            }).ToList();
            
            return Ok(tareasFormateadas);
        }

        [HttpGet("titulo/{titulo}")]
        public async Task<ActionResult<object>> GetByTitulo(string titulo)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tarea = await _service.GetByTituloAsync(usuarioNombre, titulo);
            
            if (tarea == null) return NotFound(new { mensaje = $"Tarea '{titulo}' no encontrada" });
            
            var tareaFormateada = new 
            {
                tarea.Id,
                tarea.Titulo,
                tarea.Descripcion,
                FechaCreacion = tarea.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = tarea.FechaCompletado.HasValue ? tarea.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                tarea.Completada,
                tarea.Prioridad,
                tarea.Categoria,
                tarea.UsuarioNombre
            };
            
            return Ok(tareaFormateada);
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] TareaCreateDTO dto)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaCompletado = dto.FechaCompletado,
                Prioridad = dto.Prioridad,
                Categoria = dto.Categoria,
                UsuarioNombre = usuarioNombre,
                Completada = false
            };

            var creada = await _service.CreateAsync(tarea);
            
            var respuesta = new 
            {
                creada.Id,
                creada.Titulo,
                creada.Descripcion,
                FechaCreacion = creada.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = creada.FechaCompletado.HasValue ? creada.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                creada.Completada,
                creada.Prioridad,
                creada.Categoria,
                creada.UsuarioNombre
            };
            
            return CreatedAtAction(nameof(GetByTitulo), new { titulo = creada.Titulo }, respuesta);
        }

        [HttpPut("titulo/{titulo}")]
        public async Task<IActionResult> Update(string titulo, [FromBody] TareaCreateDTO dto)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareaExistente = await _service.GetByTituloAsync(usuarioNombre, titulo);
            
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea '{titulo}' no encontrada" });

            // Verificar que el usuario es el propietario
            if (tareaExistente.UsuarioNombre != usuarioNombre)
                return Forbid();

            tareaExistente.Titulo = dto.Titulo;
            tareaExistente.Descripcion = dto.Descripcion;
            tareaExistente.FechaCompletado = dto.FechaCompletado;
            tareaExistente.Prioridad = dto.Prioridad;
            tareaExistente.Categoria = dto.Categoria;

            await _service.UpdateAsync(tareaExistente);
            return Ok(new { mensaje = "Tarea actualizada correctamente" });
        }

        [HttpDelete("titulo/{titulo}")]
        public async Task<IActionResult> Delete(string titulo)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareaExistente = await _service.GetByTituloAsync(usuarioNombre, titulo);
            
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea '{titulo}' no encontrada" });

            // Verificar que el usuario es el propietario
            if (tareaExistente.UsuarioNombre != usuarioNombre)
                return Forbid();

            await _service.DeleteAsync(tareaExistente.Id);
            return Ok(new { mensaje = "Tarea eliminada correctamente" });
        }

        [HttpPost("titulo/{titulo}/completar")]
        public async Task<IActionResult> Completar(string titulo)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareaExistente = await _service.GetByTituloAsync(usuarioNombre, titulo);
            
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea '{titulo}' no encontrada" });

            // Verificar que el usuario es el propietario
            if (tareaExistente.UsuarioNombre != usuarioNombre)
                return Forbid();

            await _service.MarcarComoCompletadaAsync(tareaExistente.Id);
            return Ok(new { mensaje = "Tarea completada correctamente" });
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<List<object>>> Buscar(
            [FromQuery] string? titulo = null, 
            [FromQuery] int? prioridad = null)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var tareas = await _service.BuscarPorTituloYPrioridadAsync(usuarioNombre, titulo, prioridad);
            
            var tareasFormateadas = tareas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                FechaCreacion = t.FechaCreacion.ToString("dd-MM-yyyy HH:mm:ss"),
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null,
                t.Completada,
                t.Prioridad,
                t.Categoria,
                t.UsuarioNombre
            }).ToList();

            return Ok(tareasFormateadas);
        }
    }
}