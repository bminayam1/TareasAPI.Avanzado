using System;
using System.Linq;
using TareasAPI.Models;

namespace TareasAPI.Helpers
{
    // Delegado para validar una tarea genérica Tarea<T>
    public delegate bool ValidarTareaDelegate<T>(Tarea<T> tarea);

    public static class ValidacionesTarea
    {
        // Valida que la descripción no esté vacía y que la fecha de vencimiento sea futura
        public static bool ValidarBasica(Tarea<string> tarea)
        {
            return !string.IsNullOrWhiteSpace(tarea.Descripcion)
                && tarea.FechaVencimiento > DateTime.Now;
        }

        // Valida que el nombre tenga al menos 1 carácter
        public static bool ValidarLongitudNombre(Tarea<string> tarea)
        {
            // Mejor validar que no sea nulo o vacío también
            return !string.IsNullOrWhiteSpace(tarea.Nombre) && tarea.Nombre.Length >= 1;
        }

        // Valida que el estado sea uno de los permitidos, comparando en minúsculas para evitar problemas de mayúsculas
        public static bool ValidarEstado(Tarea<string> tarea)
        {
            var estadosValidos = new[] { "pendiente", "completada", "en progreso" };
            return tarea.Testado != null && estadosValidos.Contains(tarea.Testado.Trim().ToLower());
        }
    }

    // Delegado para validar filtros de búsqueda
    public delegate bool ValidarFiltroDelegate(string? nombre, string? estado, DateTime? fechaVencimiento);

    public static class ValidacionesFiltro
    {
        // Valida que al menos uno de los filtros tenga valor
        public static bool ValidarCriteriosMinimos(string? nombre, string? estado, DateTime? fechaVencimiento)
        {
            // Había repetición de estado, la corregí
            return !string.IsNullOrWhiteSpace(nombre)
                || !string.IsNullOrWhiteSpace(estado)
                || fechaVencimiento.HasValue;
        }

        // Valida que la fecha esté en un rango válido
        public static bool ValidarFechaVencimiento(string? nombre, string? estado, DateTime? fechaVencimiento)
        {
            if (!fechaVencimiento.HasValue) return true;

            return fechaVencimiento.Value.Year >= 1900 && fechaVencimiento.Value.Year <= 3000;
        }
    }
}
