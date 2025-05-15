using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TareasAPI.Data;
using TareasAPI.Factories;
using TareasAPI.Helpers;
using TareasAPI.Models;
using TareasAPI.Services;

namespace TareasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : Controller
    {
        private readonly TareasContext _context;
        private readonly NotificacionesTarea _notificadora;

        public TareasController(TareasContext context, NotificacionesTarea notificadora)
        {

            _context = context;
            _notificadora = notificadora;

        }

        //POST: api/Tareas
        [HttpPost]
        public async Task<ActionResult<Tarea<string>>> CrearTarea(Tarea<string> nuevaTarea)
        {

            ValidarTareaDelegate<string> validador = ValidacionesTarea.ValidarBasica;

            if (!validador(nuevaTarea))
            {
                return BadRequest("La descripcion de la tarea esta vacia.");
            }
            var validaciones = new List<ValidarTareaDelegate<string>>
            {
                ValidacionesTarea.ValidarLongitudNombre,
                ValidacionesTarea.ValidarEstado
            };

            foreach (var validar in validaciones)
            {
                if (!validar(nuevaTarea))
                {
                    return BadRequest("Una o más validaciones fallaron, favor revisar longitud del nombre, descripción y estado.");
                }
            }

            bool tareaDuplicada = await _context.Tareas.AnyAsync(t => t.Nombre == nuevaTarea.Nombre);

            if (tareaDuplicada)
            {
                return BadRequest("Ya existe una tarea con el mismo nombre.");
            }

            _context.Tareas.Add(nuevaTarea);
            await _context.SaveChangesAsync();
            _notificadora.NotificacionCreacion(nuevaTarea);

            // Usando la clase CalculosTarea para obtener los días restantes

            int diasRestantes = CalculosTareas.CalcularDiasRestantes(nuevaTarea);

            // Retorna la tarea creada con su ID generado
            return CreatedAtAction(nameof(GetTarea), new { id = nuevaTarea.Id }, new
            {
                nuevaTarea.Id,
                nuevaTarea.Nombre,
                nuevaTarea.Descripcion,
                nuevaTarea.Testado,
                nuevaTarea.FechaVencimiento,
                DiasRestantes = diasRestantes
            });
        }
        
        //GET: api/Tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea<string>>>> GetTareas()
        {
            var tareas = await _context.Tareas.ToListAsync();

            var resultado = tareas.Select(t =>
            {
                int diasRestantes = CalculosTareas.CalcularDiasRestantes(t);

                string nivelUrgencia;
                if (diasRestantes <= 2)
                    nivelUrgencia = "Alta";
                else if (diasRestantes <= 5)
                    nivelUrgencia = "Media";
                else
                    nivelUrgencia = "Baja";

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
            });

            return Ok(resultado);
        }


        //GET: api/Tareas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea<string>>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound("La tarea no existe.");
            }

            // Usando la clase CalculosTarea para obtener los días restantes
            int diasRestantes = CalculosTareas.CalcularDiasRestantes(tarea);

            return Ok(new
            {
                tarea.Id,
                tarea.Nombre,
                DiasRestantes = diasRestantes
            });
        }


        //GET: api/Tareas/Filtrar
        [HttpGet("Filtrar")]
        public async Task<ActionResult<IEnumerable<Tarea<string>>>> FiltrarTareas(
           [FromQuery] string? nombre,
           [FromQuery] string? estado,
           [FromQuery] DateTime? fechaVencimiento)
        {
            var validaciones = new List<ValidarFiltroDelegate>
            {
                 ValidacionesFiltro.ValidarCriteriosMinimos,
                 ValidacionesFiltro.ValidarFechaVencimiento
            };

            // Ejecutar Validaciones  
            foreach (var validar in validaciones)
            {
                if (!validar(nombre, estado, fechaVencimiento))
                {
                    return BadRequest("Debe de proporcionar al menos un criterio");
                }
            }

            var query = _context.Tareas.AsQueryable();

            // Filtrar por nombres  
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(t => t.Nombre.Contains(nombre));
            }

            // Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(t => t.Testado.ToLower() == estado.ToLower());
            }

            // Filtrar por fecha de vencimiento  
            if (fechaVencimiento.HasValue)
            {
                if (fechaVencimiento.Value.Year < 1900 || fechaVencimiento.Value.Year > 3000)
                {
                    return BadRequest("La fecha de vencimiento proporcionada no es valida.");
                }

                query = query.Where(t => t.FechaVencimiento.Date == fechaVencimiento.Value.Date);
            }

            var tareasFiltradas = await query.ToListAsync();

            var resultado = tareasFiltradas.Select(t =>
            {
                int diasRestantes = CalculosTareas.CalcularDiasRestantes(t);

                string nivelUrgencia;
                if (diasRestantes <= 2)
                    nivelUrgencia = "Alta";
                else if (diasRestantes <= 5)
                    nivelUrgencia = "Media";
                else
                    nivelUrgencia = "Baja";

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
        }


        //PUT: api/Tareas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea<string> tarea)
        {
            //Validacion descripcion vacia
            if (string.IsNullOrWhiteSpace(tarea.Descripcion))
            {
                return BadRequest("La descripcion no puede estar vacia.");
            }

            if (id != tarea.Id)
            {
                return BadRequest();
            }

            _context.Entry(tarea).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            //Verificar si la tarea existe
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound("Este id de tarea no existe en la base de datos, favor intente de nuevo.");
            }

            //Eliminar tarea de la base de datos
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            _notificadora.NotificacionEliminacion(tarea);

            return NoContent();
        }
    }
}
