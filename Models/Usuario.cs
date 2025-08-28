namespace TaskCreatorAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public string Rol { get; set; } // Admin, User

        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}