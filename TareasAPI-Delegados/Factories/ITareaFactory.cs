using TareasAPI.Models;

namespace TareasAPI.Factories
{
    public interface ITareaFactory
    {
        Tarea<string> GetTarea(string nombre, string descripcion);
    }
}
