using TareasAPI.Models;

namespace TareasAPI.Factories
{
    public class TareaFactory: ITareaFactory
    {
        public virtual Tarea<string> GetTarea(string nombre, string descripcion)
        {
            return new Tarea<string>
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Testado = "Pendiente",
                FechaVencimiento = DateTime.Now.AddDays(7)
            };
        }
    }
}
