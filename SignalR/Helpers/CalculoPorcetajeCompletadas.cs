using TareasAPI.Models;

namespace TareasAPI.Helpers
{
    public class TareaService<TEstado>
    {
        public double CalcularPorcentajeCompletadas(List<Tarea<TEstado>> tareas, TEstado estadoCompletado)
        {
            tareas ??= new List<Tarea<TEstado>>();
            string key = $"porcentaje:{typeof(TEstado).Name}:{estadoCompletado?.ToString()}:{tareas.Count}";

            return Memoization.GetOrAdd(key, () =>
            {
                if (tareas.Count == 0) return 0.0;
                int completadas = tareas.Count(t => EqualityComparer<TEstado>.Default.Equals(t.Testado, estadoCompletado));
                return (double)completadas / tareas.Count * 100;
            }, TimeSpan.FromMinutes(30));
        }
    }
}
