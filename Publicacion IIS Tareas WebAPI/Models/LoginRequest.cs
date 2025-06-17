// Models/LoginRequest.cs
namespace TareasAPI.Models
{
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }
    }

}
