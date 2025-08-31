namespace TaskCreatorAPI.Models
{
    public class TareaPublicaCompletada
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime FechaCompletado { get; set; }
        public int TareaPublicaId { get; set; }
        

    }
}