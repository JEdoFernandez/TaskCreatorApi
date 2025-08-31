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
        public async Task<ActionResult<List<TareaPublica>>> Get()
        {
            var tareas = await _service.GetAllAsync();
            return Ok(tareas);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TareaPublica>> GetById(int id)
        {
            var tarea = await _service.GetByIdAsync(id);
            if (tarea == null) return NotFound(new { mensaje = $"Tarea pública con ID {id} no encontrada" });
            return Ok(tarea);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<TareaPublica>> Create([FromBody] TareaPublicaCreateDTO dto)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var tareaPublica = new TareaPublica
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Prioridad = dto.Prioridad, // ✅ Cambiado de Dificultad a Prioridad
                TiempoEstimado = dto.TiempoEstimado,
                Categoria = dto.Categoria,
                PublicadoPor = usuarioNombre,
                Recomendado = false,
                Completada = false // ✅ Siempre false al crearse
            };

            var creada = await _service.CreateAsync(tareaPublica);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TareaPublicaCreateDTO dto)
        {
            var tareaExistente = await _service.GetByIdAsync(id);
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea pública con ID {id} no encontrada" });

            // Verificar que el usuario es el propietario o admin
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var usuarioRol = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (tareaExistente.PublicadoPor != usuarioNombre && usuarioRol != "Admin")
                return Forbid();

            // Actualizar propiedades
            tareaExistente.Titulo = dto.Titulo;
            tareaExistente.Descripcion = dto.Descripcion;
            tareaExistente.Prioridad = dto.Prioridad; // ✅ Cambiado de Dificultad a Prioridad
            tareaExistente.TiempoEstimado = dto.TiempoEstimado;
            tareaExistente.Categoria = dto.Categoria;

            var actualizada = await _service.UpdateAsync(tareaExistente);
            if (!actualizada) return StatusCode(500, new { mensaje = "Error al actualizar la tarea" });

            return Ok(new { mensaje = "Tarea pública actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tareaExistente = await _service.GetByIdAsync(id);
            if (tareaExistente == null) return NotFound(new { mensaje = $"Tarea pública con ID {id} no encontrada" });

            // Verificar que el usuario es el propietario o admin
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var usuarioRol = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (tareaExistente.PublicadoPor != usuarioNombre && usuarioRol != "Admin")
                return Forbid();

            var eliminada = await _service.DeleteAsync(id);
            if (!eliminada) return StatusCode(500, new { mensaje = "Error al eliminar la tarea" });

            return Ok(new { mensaje = "Tarea pública eliminada correctamente" });
        }

        [HttpPost("{id}/completar")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Completar(int id)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var completada = await _service.MarcarComoCompletadaAsync(id, usuarioNombre);
            if (!completada) return BadRequest(new { 
                mensaje = "Ya has completado esta tara o no existe" 
            });

            return Ok(new { 
                mensaje = "Tarea completada correctamente",
                completada = true 
            });
        }

        [HttpPost("{id}/descompletar")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Descompletar(int id)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var descompletada = await _service.DesmarcarComoCompletadaAsync(id, usuarioNombre);
            if (!descompletada) return BadRequest(new { 
                mensaje = "No habías completado esta tarea o no existe" 
            });

            return Ok(new { 
                mensaje = "Tarea descompletada correctamente",
                completada = false 
            });
        }

        [HttpGet("completadas")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublicaCompletada>>> GetCompletadas()
        {
            var completadas = await _completadaService.GetAllAsync();
            return Ok(completadas);
        }

        [HttpGet("mis-completadas")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<List<TareaPublicaCompletada>>> GetMisCompletadas()
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            var completadas = await _completadaService.GetByUsuarioNombreAsync(usuarioNombre);
            return Ok(completadas);
        }

        [HttpGet("buscar/{titulo}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublica>>> Buscar(string titulo)
        {
            var tareas = await _service.BuscarPorTituloAsync(titulo);
            return Ok(tareas);
        }

        // ✅ NUEVO ENDPOINT: Obtener tareas públicas de un usuario específico
        [HttpGet("usuario/{nombreUsuario}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublica>>> GetByUsuario(string nombreUsuario)
        {
            var todasTareas = await _service.GetAllAsync();
            var tareasUsuario = todasTareas
                .Where(t => t.PublicadoPor.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(tareasUsuario);
        }

        // ✅ NUEVO ENDPOINT: Obtener tareas por categoría
        [HttpGet("categoria/{categoria}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublica>>> GetByCategoria(string categoria)
        {
            var todasTareas = await _service.GetAllAsync();
            var tareasCategoria = todasTareas
                .Where(t => t.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(tareasCategoria);
        }

        // ✅ NUEVO ENDPOINT: Obtener tareas por prioridad
        [HttpGet("prioridad/{prioridad}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublica>>> GetByPrioridad(int prioridad)
        {
            var todasTareas = await _service.GetAllAsync();
            var tareasPrioridad = todasTareas
                .Where(t => t.Prioridad == prioridad)
                .ToList();

            return Ok(tareasPrioridad);
        }

        // ✅ NUEVO ENDPOINT: Obtener tareas completadas/incompletas
        [HttpGet("estado/{completada}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TareaPublica>>> GetByEstado(bool completada)
        {
            var todasTareas = await _service.GetAllAsync();
            var tareasEstado = todasTareas
                .Where(t => t.Completada == completada)
                .ToList();

            return Ok(tareasEstado);
        }
    }
}