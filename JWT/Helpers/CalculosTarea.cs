using TareasAPI.Models;


namespace TareasAPI.Helpers
{
    //Se calculan los dias restantes de las tareas utilizando Func
    public static class CalculosTareas
    {
        public static readonly Func<Tarea<string>, int> CalcularDiasRestantes = (tarea) =>
        (tarea.FechaVencimiento.Date - DateTime.Now.Date).Days;
    }
}
