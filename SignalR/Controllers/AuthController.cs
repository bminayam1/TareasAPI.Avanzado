
using Microsoft.AspNetCore.Mvc;
using TareasAPI.Models;
using TareasAPI.Services;

namespace TareasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simular validación (deberías usar base de datos real)
            if (request.Usuario == "admin" && request.Contrasena == "admin123")
            {
                var token = _jwtService.GenerarToken(request.Usuario, "Administrador");
                return Ok(new { token });
            }

            return Unauthorized("Credenciales inválidas.");
        }
    }
}
