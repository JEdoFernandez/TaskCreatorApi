namespace TaskCreatorAPI.Models
{
    public class TareaPublica
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string PublicadoPor { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Recomendado { get; set; }
        public int Prioridad { get; set; } 
        public int TiempoEstimado { get; set; } 
        public string Categoria { get; set; }
        public bool Completada { get; set; } = false;
        
    
    }
}