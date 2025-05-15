using TareasAPI.Models;

namespace TareasAPI.Factories
{
    public class TareasAltaPrioridadFactory: TareaFactory
    {
        public override Tarea<string> CrearTarea(string nombre, string descripcion)
        {
           var tarea = base.CrearTarea(nombre, descripcion);
            tarea.Testado = "Pendiente";
            tarea.FechaVencimiento = DateTime.Now.AddDays(7);
            return tarea;
        }
    }
}
