namespace TaskCreatorAPI.Models.DTOs
{
    public class TareaCreateDTO
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaLimite { get; set; }
        public int Prioridad { get; set; }
        public string Categoria { get; set; }
    }
}