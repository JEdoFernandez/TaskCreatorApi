using Microsoft.AspNetCore.Mvc;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Models.DTOs;

namespace TaskCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                var token = _authService.Authenticate(dto.Nombre, dto.Contraseña);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistroDTO dto)
        {
            try
            {
                _authService.Register(dto.Nombre, dto.Email, dto.Contraseña);
                return Ok(new { mensaje = "Usuario registrado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }
    }
}