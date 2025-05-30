using TareasAPI.Models;
using System.Collections.Generic;
using System.Linq;
using TareasAPI.Helpers;

namespace TareasAPI.Services
{
    public class TareaService<TEstado>
    {
        public double CalcularPorcentajeCompletadas(List<Tarea<TEstado>> tareas, TEstado estadoCompletado)
        {
            var keyPart = string.Join("_", tareas.Select(t => t.Id).OrderBy(id => id));
            string cacheKey = $"porcentaje_{estadoCompletado}_{keyPart}";

            return Memoization.GetOrAdd(cacheKey, () =>
            {
                if (tareas.Count == 0) return 0;

                int completadas = tareas.Count(t => EqualityComparer<TEstado>.Default.Equals(t.Testado, estadoCompletado));
                return (double)completadas / tareas.Count * 100;
            });
        }
    }

}