using System.ComponentModel.DataAnnotations;

namespace TareasAPI.Models
{
    public class Tarea<TEstado>
    {
        
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; }
        
    
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        public TEstado Testado { get; set; }
    }
}
