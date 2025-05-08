using TareasAPI.Models;

namespace TareasAPI.Helpers
{
    public delegate bool ValidarTareaDelegate<T>(Tarea<T> tarea);

    public static class ValidacionesTarea
    {
        public static bool ValidarBasica(Tarea<string> tarea)
        {
            return !string.IsNullOrWhiteSpace(tarea.Descripcion)
                && tarea.FechaVencimiento > DateTime.Now;
        }
        public static bool ValidarLongitudNombre(Tarea<string> tarea)
        {
            return tarea.Nombre.Length >= 1;
        }
        public static bool ValidarEstado(Tarea<string> tarea)
        {
            var estadosValidos = new[] { "Pendiente", "Completada", "En Progreso" };
            return estadosValidos.Contains(tarea.Testado);
        }
    }
    public delegate bool ValidarFiltroDelegate(string? nombre, string? estado, DateTime? fechaVencimiente);
    public static class ValidacionesFiltro
    {
        public static bool ValidarCriteriosMinimos(string? nombre, string? estado, DateTime? fechaVencimiento)
        {
            return !string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(estado) || !string.IsNullOrWhiteSpace(estado) || fechaVencimiento.HasValue;
        }
        public static bool ValidarFechaVencimiento(string? nombre, string? estado, DateTime? fechaVencimiento)
        {
            if (!fechaVencimiento.HasValue) return true;
            return fechaVencimiento.Value.Year >= 1900 && fechaVencimiento.Value.Year <= 3000;
        
        }
    }

}
