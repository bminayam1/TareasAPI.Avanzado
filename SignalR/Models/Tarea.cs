using System.ComponentModel.DataAnnotations;

namespace TareasAPI.Models
{
    public class Tarea<TEstado>
    {
        
        public int Id { get; set; }

       
        [MaxLength(50)]
        public string Nombre { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }
        
    
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        public TEstado Testado { get; set; }
    }
}
