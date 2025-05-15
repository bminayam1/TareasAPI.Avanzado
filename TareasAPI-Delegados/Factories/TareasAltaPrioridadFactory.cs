using TareasAPI.Models;

namespace TareasAPI.Factories
{
    public class TareasAltaPrioridadFactory: TareaFactory
    {
        public override Tarea<string> GetTarea(string nombre, string descripcion)
        {
           var tarea = base.GetTarea(nombre, descripcion);
            tarea.Testado = "Pendiente";
            tarea.FechaVencimiento = DateTime.Now.AddDays(7);
            return tarea;
        }
    }
}
