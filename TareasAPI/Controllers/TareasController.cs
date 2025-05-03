using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasAPI.Data;
using TareasAPI.Models;

namespace TareasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : Controller
    {
        private readonly TareasContext _context;

        public object ObtenerTareaPorId { get; private set; }

        public TareasController(TareasContext context)
        {
            {
                _context = context;
            }
        }
        //POST: api/Tareas
        [HttpPost]
        public async Task<ActionResult<Tarea<string>>> CrearTarea(Tarea<string> nuevaTarea)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(nuevaTarea.Descripcion))
            {
                return BadRequest("La descripcion no puede estar vacia.");
            }

            if (nuevaTarea.FechaVencimiento <= DateTime.Now)
            {
                return BadRequest("La fecha de vencimiento debe ser futura.");
            }

            bool tareaDuplicada = await _context.Tareas.AnyAsync(t => t.Nombre == nuevaTarea.Nombre);
            
            if (tareaDuplicada)
            {
                return BadRequest("Ya existe una tarea con el mismo nombre.");
            }

            _context.Tareas.Add(nuevaTarea);
            await _context.SaveChangesAsync();

            // Retorna la tarea creada con su ID generado
            return CreatedAtAction(nameof(GetTarea), new { id = nuevaTarea.Id }, nuevaTarea);
        }

        //GET: api/Tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea<string>>>> GetTareas()
        {
            return await _context.Tareas.ToListAsync();
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

            return tarea;
        }

        //GET: api/Tareas/{Filtrar}
        [HttpGet("Filtrar")]
        public async Task<ActionResult<IEnumerable<Tarea<string>>>> FiltrarTareas(

            [FromQuery] string? nombre,
            [FromQuery] string? estado,
            [FromQuery] DateTime? fechaVencimiento)
        {
            var query = _context.Tareas.AsQueryable();
            
            if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(estado) && !fechaVencimiento.HasValue)
            {
                return BadRequest("Debe proporcionar al menos un criterio de filtro (nombre, estado o fecha de vencimiento).");
            }

            if (!string.IsNullOrEmpty(nombre))
            {

                query = query.Where(t => t.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(estado))
            {

                query = query.Where(t => t.Testado == estado);
            }

            if (fechaVencimiento.HasValue)
            {
                if (fechaVencimiento.Value.Year <1900 || fechaVencimiento.Value.Year > 3000)
                {
                    return BadRequest("La fecha de vencimiento proporcionada no es valida.");
                }
                
                query = query.Where(t => t.FechaVencimiento.Date == fechaVencimiento.Value.Date);
            }

            var resultado = await query.ToListAsync();
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
                return NotFound();
            }

            //Eliminar tarea de la base de datos
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
