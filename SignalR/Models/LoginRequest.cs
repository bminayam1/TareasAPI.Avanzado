﻿// Models/LoginRequest.cs
namespace TareasAPI.Models
{
    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
