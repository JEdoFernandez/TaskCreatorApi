namespace TaskCreatorAPI.Models.DTOs
{
    public class TareaPublicaCreateDTO
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Dificultad { get; set; }
        public int TiempoEstimado { get; set; }
        public string Categoria { get; set; }
    }
}