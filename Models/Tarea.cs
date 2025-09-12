namespace TaskCreatorAPI.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCompletado { get; set; }  
        public bool Completada { get; set; }
        public int Prioridad { get; set; } 
        public string Categoria { get; set; }
        
        public string UsuarioNombre { get; set; } 
    }
}