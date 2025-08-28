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
            if (tarea == null) return NotFound();
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
                Dificultad = dto.Dificultad,
                TiempoEstimado = dto.TiempoEstimado,
                Categoria = dto.Categoria,
                PublicadoPor = usuarioNombre,
                Recomendado = false
            };

            var creada = await _service.CreateAsync(tareaPublica);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }

        [HttpPost("{id}/completar")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Completar(int id)
        {
            var usuarioNombre = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var completada = await _service.MarcarComoCompletadaAsync(id, usuarioNombre);
            if (!completada) return BadRequest("Ya has completado esta tarea");

            return NoContent();
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
    }
}