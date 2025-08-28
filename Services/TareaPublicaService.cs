using TaskCreatorAPI.Data.Repositories;
using TaskCreatorAPI.Models;

namespace TaskCreatorAPI.Services
{
    public class TareaPublicaService
    {
        private readonly TareaPublicaRepository _repository;
        private readonly TareaPublicaCompletadaRepository _completadaRepository;

        public TareaPublicaService(TareaPublicaRepository repository, TareaPublicaCompletadaRepository completadaRepository)
        {
            _repository = repository;
            _completadaRepository = completadaRepository;
        }

        public async Task<List<TareaPublica>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TareaPublica> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<TareaPublica> CreateAsync(TareaPublica tareaPublica)
        {
            tareaPublica.FechaPublicacion = DateTime.Now;
            tareaPublica.VecesCompletada = 0;
            return await _repository.AddAsync(tareaPublica);
        }

        public async Task<bool> UpdateAsync(TareaPublica tareaPublica)
        {
            return await _repository.UpdateAsync(tareaPublica);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<TareaPublica>> BuscarPorTituloAsync(string titulo)
        {
            return await _repository.BuscarPorTituloAsync(titulo);
        }

        public async Task<bool> MarcarComoCompletadaAsync(int tareaPublicaId, string usuarioNombre)
        {
            // Verificar si ya fue completada
            if (await _completadaRepository.ExisteCompletadaAsync(tareaPublicaId, usuarioNombre))
                return false;

            var tareaPublica = await _repository.GetByIdAsync(tareaPublicaId);
            if (tareaPublica == null) return false;

            // Crear registro de completada
            var tareaCompletada = new TareaPublicaCompletada
            {
                Titulo = tareaPublica.Titulo,
                Descripcion = tareaPublica.Descripcion,
                UsuarioNombre = usuarioNombre,
                FechaCompletado = DateTime.Now,
                PuntosObtenidos = tareaPublica.Dificultad * 10, // 10 puntos por nivel de dificultad
                TareaPublicaId = tareaPublicaId
            };

            await _completadaRepository.AddAsync(tareaCompletada);
            await _repository.IncrementarVecesCompletadaAsync(tareaPublicaId);

            return true;
        }
    }
}