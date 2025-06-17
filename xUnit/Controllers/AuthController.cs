using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasAPI.Data;
using TareasAPI.Models;
using TareasAPI.Services;
using BCrypt.Net;

namespace TareasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly TareasContext _context;

        public AuthController(JwtService jwtService, TareasContext context) // Se agregó _context aquí
        {
            _jwtService = jwtService;
            _context = context; // Se inicializa correctamente
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Usuario);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
                return Unauthorized("Credenciales inválidas.");

            var token = _jwtService.GenerarToken(usuario.Nombre, "Usuario");
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
            if (existe) return BadRequest("El usuario ya existe.");

            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario creado exitosamente.");
        }
    }
}
