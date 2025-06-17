using TareasAPI.Models;

namespace TareasAPI.Repositories
{
    public interface ITareaRepositorio
    {
        void AgregarTareas<T>(Tarea<T> tarea);
    }

}
