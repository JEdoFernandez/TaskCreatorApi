using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Services
{
    public class AuthService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(UsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public string Authenticate(string nombre, string contraseña)
        {
            var usuario = _usuarioRepository.GetByNombreAsync(nombre).Result;
            if (usuario == null || usuario.Contraseña != contraseña)
                throw new UnauthorizedAccessException("Credenciales incorrectas");

            return GenerateJwtToken(usuario);
        }

        public void Register(string nombre, string email, string contraseña, string rol = "User")
        {
            var usuarioExistente = _usuarioRepository.GetByNombreAsync(nombre).Result;
            if (usuarioExistente != null)
                throw new ArgumentException("El nombre de usuario ya está en uso");

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Contraseña = contraseña,
                FechaRegistro = DateTime.Now,
                Activo = true,
                Rol = rol // Usar el rol proporcionado
            };

            _usuarioRepository.AddAsync(nuevoUsuario).Wait();
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}