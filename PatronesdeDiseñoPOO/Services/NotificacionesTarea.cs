using System;
using TareasAPI.Models;
using Microsoft.Extensions.Logging;

namespace TareasAPI.Services
{
    public class NotificacionesTarea
    {
        private ILogger<NotificacionesTarea> _logger;

        public NotificacionesTarea(ILogger<NotificacionesTarea> logger)
        {
            _logger = logger;
        }
        public Action<Tarea<string>> NotificacionCreacion => tarea =>
        {
            _logger.LogInformation($"✅Tarea creada correctamente: {tarea.Nombre} - Fecha: {tarea.FechaVencimiento}");
        };
        public Action<Tarea<string>> NotificacionEliminacion => tarea =>
        {
            _logger.LogWarning($"❌Tarea eliminada: {tarea.Nombre} - ID: {tarea.Id}");
        };
    }
}
