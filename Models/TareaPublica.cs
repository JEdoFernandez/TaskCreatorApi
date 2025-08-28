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
        public int Dificultad { get; set; } // 1-5
        public int TiempoEstimado { get; set; } // en minutos
        public string Categoria { get; set; }
        public int VecesCompletada { get; set; }

        public List<TareaPublicaCompletada> Completadas { get; set; } = new List<TareaPublicaCompletada>();
    }
}