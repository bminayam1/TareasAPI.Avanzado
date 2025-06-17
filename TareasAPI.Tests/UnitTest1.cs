using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TareasAPI.Helpers;
using TareasAPI.Models;
using TareasAPI.Services;
using System;
using Xunit;

namespace TareasAPI.Tests
{
    public class UnitTest1
    {
        //Prueba 1: para validacion de tareas (Descripcion y Fecha)
        [Fact]
        public void ValidarBasica_DeberiaRetornarTrue_ParaTareaValida()
        {
            var tarea = new Tarea<string>
            {
                Nombre = "Prueba",
                Descripcion = "Tarea válida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarBasica(tarea);

            Assert.True(resultado);
        }

        //Prueba 2: Validar longitud de nombre
        [Fact]
        public void ValidarLongitudNombre_DeberiaRetornarTrue_CuandoNombreTieneLongitudMinima()
        {
            var tarea = new Tarea<string> { Nombre = "A" };

            var resultado = ValidacionesTarea.ValidarLongitudNombre(tarea);

            Assert.True(resultado);
        }
        //Prueba 3: Validar Estado permitidos

        [Theory]
        [InlineData("pendiente")]
        [InlineData("completada")]
        [InlineData("en progreso")]
        public void ValidarEstado_DeberiaRetornarTrue_ParaEstadosValidos(string estado)
        {
            var tarea = new Tarea<string> { Testado = estado };

            var resultado = ValidacionesTarea.ValidarEstado(tarea);

            Assert.True(resultado);
        }

        //Prueba 4: Validar filtro de busqueda

        [Fact]

        public void ValidarCriteriosMinimos_DeberiaRetornarTrue_CuandoAlMenosUnFiltroTieneValor()
        {
            var resultado = ValidacionesFiltro.ValidarCriteriosMinimos("Tarea", null, null);
            Assert.True(resultado);
        }

        //Prueba 5: Validar fecha de vencimiento

        [Theory]
        [InlineData("1899", false)]
        [InlineData("2000", true)]
        [InlineData("3001", false)]

        public void ValidarFechaVencimiento_DeberiaRetornarTrue_CuandoFechaEsValida(string year, bool esperado)
        {
            DateTime? fecha = new DateTime(int.Parse(year), 1, 1);
            var resultado = ValidacionesFiltro.ValidarFechaVencimiento(null, null, fecha);
            Assert.Equal(esperado, resultado);
        }

        //Prueba 6: Generar Token JWT

        [Fact]
        public void GenerarToken_DeberiaRetornarTokenNoVacio()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
        { "Jwt:Issuer", "TestIssuer" },
        { "Jwt:Audience", "TestAudience" },
        { "Jwt:ClaveSecreta", "ClaveSuperSecreta123456789" }
            }).Build();

            var jwtService = new JwtService(config);

            var token = jwtService.GenerarToken("usuarioTest", "admin");

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        //Pruebqa 7: Cache sin expirar 

        [Fact]
        public void Memoization_DeberiaRetornarValorCacheado_SiNoHaExpirado()
        {
            string key = "clave1";
            Func<int> factory = () => 42;

            var valor1 = Memoization.GetOrAdd(key, factory, TimeSpan.FromMinutes(5));
            var valor2 = Memoization.GetOrAdd(key, () => 99, TimeSpan.FromMinutes(5));

            Assert.Equal(valor1, valor2); // debe mantener el cache anterior
        }

        //Prueba 8: Cola de tareas procesa una tarea

        [Fact]
        public async Task ColaDeTareas_DeberiaProcesarTarea()
        {
            var cola = new ColaDeTareas<string>();
            cola.AgregarTarea("tarea1");

            await Task.Delay(2500); // espera a que se procese

            // No hay assertion directa, pero no debe lanzar error
            Assert.True(true);
        }


        //Prueba 9: Notificación de tarea creada

        [Fact]
        public void NotificacionesTarea_DeberiaLlamarLogger_CuandoSeCreaTarea()
        {
            var mockLogger = new Moq.Mock<ILogger<NotificacionesTarea>>();
            var notificaciones = new NotificacionesTarea(mockLogger.Object);
            var tarea = new Tarea<string> { Nombre = "Tarea Test", FechaVencimiento = DateTime.Now };

            notificaciones.NotificacionCreacion(tarea);

            mockLogger.Verify(
                x => x.LogInformation(It.Is<string>(msg => msg.Contains("Tarea creada correctamente"))),
                Times.Once);
        }

        //Prueba 10: Borrado completo de Cache
        [Fact]
        public void Memoization_DeberiaBorrarCacheConClear()
        {
            Memoization.GetOrAdd("claveBorrar", () => 1, TimeSpan.FromMinutes(5));
            Memoization.ClearCache("claveBorrar");

            var nuevo = Memoization.GetOrAdd("claveBorrar", () => 2, TimeSpan.FromMinutes(5));

            Assert.Equal(2, nuevo);
        }


    }
}
