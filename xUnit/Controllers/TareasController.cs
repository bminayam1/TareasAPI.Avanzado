using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TareasAPI.Data;
using TareasAPI.Hubs;
using TareasAPI.Models;
using TareasAPI.Services;
using TareasAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TareasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TareasContext _context;
        private readonly NotificacionesTarea _notificadora;
        private readonly IHubContext<TareasHubs> _hubContext;

        private static readonly ColaDeTareas<Tarea<string>> _colaDeTareas = new ColaDeTareas<Tarea<string>>();

        public TareasController(
            TareasContext context,
            NotificacionesTarea notificadora,
            IHubContext<TareasHubs> hubContext)
        {
            _context = context;
            _notificadora = notificadora;
            _hubContext = hubContext;
        }

       
        [HttpPost]
        public async Task<ActionResult<Tarea<string>>> CrearTarea(Tarea<string> nuevaTarea)
        {
            // Validaciones básicas
            ValidarTareaDelegate<string> validador = ValidacionesTarea.ValidarBasica;
            if (!validador(nuevaTarea))
                return BadRequest("La descripción está vacía o la fecha de vencimiento es inválida.");

            var validaciones = new List<ValidarTareaDelegate<string>>
            {
                ValidacionesTarea.ValidarLongitudNombre,
                ValidacionesTarea.ValidarEstado
            };

            foreach (var validar in validaciones)
            {
                if (!validar(nuevaTarea))
                    return BadRequest("Una o más validaciones fallaron: longitud del nombre, descripción o estado inválido.");
            }

            bool tareaDuplicada = await _context.Tareas.AnyAsync(t => t.Nombre == nuevaTarea.Nombre);
            if (tareaDuplicada)
                return BadRequest("Ya existe una tarea con el mismo nombre.");

            // Agregar tarea a la BD
            _context.Tareas.Add(nuevaTarea);
            await _context.SaveChangesAsync();

            // Notificaciones
            _notificadora.NotificacionCreacion(nuevaTarea);

            // Enviar notificación en tiempo real a clientes via SignalR
            await _hubContext.Clients.All.SendAsync("NuevaTareaCreada", new
            {
                nuevaTarea.Id,
                nuevaTarea.Nombre,
                nuevaTarea.Descripcion,
                nuevaTarea.FechaVencimiento
            });

            // Agregar tarea a la cola para procesamiento
            _colaDeTareas.AgregarTarea(nuevaTarea);

            // Cálculo de días restantes y nivel de urgencia
            int diasRestantes = CalculosTareas.CalcularDiasRestantes(nuevaTarea);
            string nivelUrgencia = diasRestantes <= 2 ? "Alta" : diasRestantes <= 5 ? "Media" : "Baja";

            // Retornar resultado con detalles
            return CreatedAtAction(nameof(GetTarea), new { id = nuevaTarea.Id }, new
            {
                nuevaTarea.Id,
                nuevaTarea.Nombre,
                nuevaTarea.Descripcion,
                nuevaTarea.Testado,
                nuevaTarea.FechaVencimiento,
                DiasRestantes = diasRestantes,
                NivelUrgencia = nivelUrgencia
            });
        }

        // GET: api/Tareas
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTareas()
        {
            return await Memoization.GetOrAddAsync("GetTareasCache", async () =>
            {
                var tareas = await _context.Tareas.ToListAsync();

                var resultado = tareas.Select(t =>
                {
                    int diasRestantes = CalculosTareas.CalcularDiasRestantes(t);
                    string nivelUrgencia = diasRestantes <= 2 ? "Alta" :
                                           diasRestantes <= 5 ? "Media" : "Baja";

                    return new
                    {
                        t.Id,
                        t.Nombre,
                        t.Descripcion,
                        t.Testado,
                        t.FechaVencimiento,
                        DiasRestantes = diasRestantes,
                        NivelUrgencia = nivelUrgencia
                    };
                }).ToList();

                return Ok(resultado);
            }, TimeSpan.FromMinutes(30));
        }

        // GET: api/Tareas/{id}
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                return NotFound("La tarea no existe.");

            int diasRestantes = CalculosTareas.CalcularDiasRestantes(tarea);

            return Ok(new
            {
                tarea.Id,
                tarea.Nombre,
                DiasRestantes = diasRestantes
            });
        }

        // GET: api/Tareas/Filtrar
        //[Authorize]
        [HttpGet("Filtrar")]
        public async Task<ActionResult<IEnumerable<object>>> FiltrarTareas(
            [FromQuery] string? nombre,
            [FromQuery] string? estado,
            [FromQuery] DateTime? fechaVencimiento)
        {
            string cacheKey = $"Filtro_{nombre}_{estado}_{fechaVencimiento?.ToString("yyyyMMdd") ?? "null"}";

            return await Memoization.GetOrAddAsync(cacheKey, async () =>
            {
                var query = _context.Tareas.AsQueryable();
                if (!string.IsNullOrEmpty(nombre)) query = query.Where(t => t.Nombre.Contains(nombre));
                if (!string.IsNullOrEmpty(estado)) query = query.Where(t => t.Testado.ToLower() == estado.ToLower());
                if (fechaVencimiento.HasValue) query = query.Where(t => t.FechaVencimiento.Date == fechaVencimiento.Value.Date);

                var tareasFiltradas = await query.ToListAsync();

                var resultado = tareasFiltradas.Select(t =>
                {
                    int diasRestantes = CalculosTareas.CalcularDiasRestantes(t);
                    string nivelUrgencia = diasRestantes <= 2 ? "Alta" : diasRestantes <= 5 ? "Media" : "Baja";

                    return new
                    {
                        t.Id,
                        t.Nombre,
                        t.Descripcion,
                        t.Testado,
                        t.FechaVencimiento,
                        DiasRestantes = diasRestantes,
                        NivelUrgencia = nivelUrgencia
                    };
                }).ToList();

                return Ok(resultado);
            }, TimeSpan.FromMinutes(30));
        }

        // PUT: api/Tareas/{id}
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea<string> tarea)
        {
            if (string.IsNullOrWhiteSpace(tarea.Descripcion))
                return BadRequest("La descripción no puede estar vacía.");

            if (id != tarea.Id)
                return BadRequest("El ID de la tarea no coincide con el ID de la URL.");

            _context.Entry(tarea).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Tareas/{id}
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                return NotFound("Este ID de tarea no existe en la base de datos, favor intente de nuevo.");

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            _notificadora.NotificacionEliminacion(tarea);

            return NoContent();
        }
    }
}
