using TareasAPI.Models;
using TareasAPI.Repositories;

namespace TareasAPI.Services
{
    public class ServicioTarea
    {
        private readonly ITareaRepositorio _repositorio;

        public ServicioTarea(ITareaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public void CrearTarea<T>(Tarea<T> tarea)
        {
            _repositorio.AgregarTareas(tarea);
        }
    }

}
