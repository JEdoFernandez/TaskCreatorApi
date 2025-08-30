namespace TaskCreatorAPI.Models.DTOs
{
    public class UsuarioUpdateDTO
    {
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public bool Activo { get; set; }
        public string Rol { get; set; }
    }
}