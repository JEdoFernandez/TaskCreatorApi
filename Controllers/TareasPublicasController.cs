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
    public class TareasPublicasController : ControllerBase
    {
        private readonly TareaPublicaService _service;
        private readonly TareaPublicaCompletadaService _completadaService;

        public TareasPublicasController(TareaPublicaService service, TareaPublicaCompletadaService completadaService)
        {
            _service = service;
            _completadaService = completadaService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<object>>> Get()
        {
            var tareas = await _service.GetAllAsync();
            var tareasFormateadas = tareas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                t.PublicadoPor,
                FechaPublicacion = t.FechaPublicacion.ToString("dd-MM-yyyy HH:mm:ss"),
                t.Recomendado,
                t.Prioridad,
                t.Categoria,
                t.Completada,
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null
            }).ToList();
            
            return Ok(tareasFormateadas);
        }

        [HttpGet("titulo/{titulo}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetByTitulo(string titulo)
        {
            var tareas = await _service.BuscarPorTituloAsync(titulo);
            var tarea = tareas.FirstOrDefault();
            
            if (tarea == null) return NotFound(new { mensaje = $"Tarea pública '{titulo}' no encontrada" });
            
            var tareaFormateada = new 
            {
                tarea.Id,
                tarea.Titulo,
                tarea.Descripcion,
                tarea.PublicadoPor,
                FechaPublicacion = tarea.FechaPublicacion.ToString("dd-MM-yyyy HH:mm:ss"),
                tarea.Recomendado,
                tarea.Prioridad,
                tarea.Categoria,
                tarea.Completada,
                FechaCompletado = tarea.FechaCompletado.HasValue ? tarea.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null
            };
            
            return Ok(tareaFormateada);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<object>> Create([FromBody] TareaPublicaCreateDTO dto)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var tareaPublica = new TareaPublica
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Prioridad = dto.Prioridad,
                Categoria = dto.Categoria,
                PublicadoPor = usuarioNombre,
                Recomendado = false,
                Completada = false,
                FechaCompletado = null
            };

            var creada = await _service.CreateAsync(tareaPublica);
            
            var respuesta = new 
            {
                creada.Id,
                creada.Titulo,
                creada.Descripcion,
                creada.PublicadoPor,
                FechaPublicacion = creada.FechaPublicacion.ToString("dd-MM-yyyy HH:mm:ss"),
                creada.Recomendado,
                creada.Prioridad,
                creada.Categoria,
                creada.Completada,
                FechaCompletado = creada.FechaCompletado.HasValue ? creada.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null
            };
            
            return CreatedAtAction(nameof(GetByTitulo), new { titulo = creada.Titulo }, respuesta);
        }

        [HttpPut("titulo/{titulo}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(string titulo, [FromBody] TareaPublicaCreateDTO dto)
        {
            var tareas = await _service.BuscarPorTituloAsync(titulo);
            var tareaExistente = tareas.FirstOrDefault();
            
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea pública '{titulo}' no encontrada" });

            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var usuarioRol = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (tareaExistente.PublicadoPor != usuarioNombre && usuarioRol != "Admin")
                return Forbid();

            tareaExistente.Titulo = dto.Titulo;
            tareaExistente.Descripcion = dto.Descripcion;
            tareaExistente.Prioridad = dto.Prioridad;
            tareaExistente.Categoria = dto.Categoria;

            var actualizada = await _service.UpdateAsync(tareaExistente);
            if (!actualizada) return StatusCode(500, new { mensaje = "Error al actualizar la tarea" });

            return Ok(new { mensaje = "Tarea pública actualizada correctamente" });
        }

        [HttpDelete("titulo/{titulo}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(string titulo)
        {
            var tareas = await _service.BuscarPorTituloAsync(titulo);
            var tareaExistente = tareas.FirstOrDefault();
            
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea pública '{titulo}' no encontrada" });

            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var usuarioRol = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (tareaExistente.PublicadoPor != usuarioNombre && usuarioRol != "Admin")
                return Forbid();

            var eliminada = await _service.DeleteAsync(tareaExistente.Id);
            if (!eliminada) return StatusCode(500, new { mensaje = "Error al eliminar la tarea" });

            return Ok(new { mensaje = "Tarea pública eliminada correctamente" });
        }

        [HttpPost("titulo/{titulo}/completar")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Completar(string titulo)
        {
            var tareas = await _service.BuscarPorTituloAsync(titulo);
            var tarea = tareas.FirstOrDefault();
            
            if (tarea == null) return NotFound(new { mensaje = $"Tarea pública '{titulo}' no encontrada" });

            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var completada = await _service.MarcarComoCompletadaAsync(tarea.Id, usuarioNombre);
            if (!completada) return BadRequest(new { 
                mensaje = "Ya has completado esta tarea o no existe" 
            });

            return Ok(new { 
                mensaje = "Tarea completada correctamente",
                completada = true 
            });
        }

        [HttpGet("completadas")]
        [AllowAnonymous]
        public async Task<ActionResult<List<object>>> GetCompletadas()
        {
            var completadas = await _completadaService.GetAllAsync();
            var completadasFormateadas = completadas.Select(c => new 
            {
                c.Id,
                c.Titulo,
                c.Descripcion,
                c.UsuarioNombre,
                FechaCompletado = c.FechaCompletado.ToString("dd-MM-yyyy HH:mm:ss"),
                c.TareaPublicaId
            }).ToList();
            
            return Ok(completadasFormateadas);
        }

        [HttpGet("mis-completadas")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<List<object>>> GetMisCompletadas()
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var completadas = await _completadaService.GetByUsuarioNombreAsync(usuarioNombre);
            
            var completadasFormateadas = completadas.Select(c => new 
            {
                c.Id,
                c.Titulo,
                c.Descripcion,
                c.UsuarioNombre,
                FechaCompletado = c.FechaCompletado.ToString("dd-MM-yyyy HH:mm:ss"),
                c.TareaPublicaId
            }).ToList();
            
            return Ok(completadasFormateadas);
        }

        [HttpGet("buscar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<object>>> Buscar(
            [FromQuery] string? titulo = null, 
            [FromQuery] int? prioridad = null)
        {
            var todasTareas = await _service.GetAllAsync();
            var tareasFiltradas = todasTareas.AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
            {
                tareasFiltradas = tareasFiltradas.Where(t => 
                    t.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
            }

            if (prioridad.HasValue)
            {
                tareasFiltradas = tareasFiltradas.Where(t => t.Prioridad == prioridad.Value);
            }

            var tareasFormateadas = tareasFiltradas.Select(t => new 
            {
                t.Id,
                t.Titulo,
                t.Descripcion,
                t.PublicadoPor,
                FechaPublicacion = t.FechaPublicacion.ToString("dd-MM-yyyy HH:mm:ss"),
                t.Recomendado,
                t.Prioridad,
                t.Categoria,
                t.Completada,
                FechaCompletado = t.FechaCompletado.HasValue ? t.FechaCompletado.Value.ToString("dd-MM-yyyy HH:mm:ss") : null
            }).ToList();

            return Ok(tareasFormateadas);
        }
    }
}