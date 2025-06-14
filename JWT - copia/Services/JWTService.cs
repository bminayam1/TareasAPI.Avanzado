
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TareasAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(string usuario, string rol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario),
                new Claim(ClaimTypes.Role, rol)
            };

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:ClaveSecreta"]!));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
