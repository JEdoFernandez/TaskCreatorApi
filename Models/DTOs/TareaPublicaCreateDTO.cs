namespace TaskCreatorAPI.Models.DTOs
{
    public class TareaPublicaCreateDTO
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; } // 1-5
        public string Categoria { get; set; }
    }
}