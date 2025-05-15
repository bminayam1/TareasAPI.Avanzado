using TareasAPI.Models;

namespace TareasAPI.Factories
{
    public interface ITareaFactory
    {
        Tarea<string> CrearTarea(string nombre, string descripcion);
    }
}
