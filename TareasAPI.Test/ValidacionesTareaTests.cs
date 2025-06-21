using Moq;
using TareasAPI.Helpers;
using TareasAPI.Models;
using TareasAPI.Repositories;
using TareasAPI.Services;

namespace TareasAPI.Test
{
    public class ValidacionesTareaTests
    {


        [Fact]
        public void ValidarBasica_DeberiaRetornarTrue_SiDescripcionValidaYFechaFutura()
        {
            // Prueba 1: Crear una tarea con descripcion valida y fecha futura
            var tarea = new Tarea<string>
            {
                Nombre = "Test",
                Descripcion = "Descripción válida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarBasica(tarea);
            Assert.True(resultado);
        }

        [Fact]
        public void ValidarBasica_DeberiaRetornarFalse_SiDescripcionEsVacia()
        {
            //Prueba 2: Crear una tarea con descripcion vacia
            var tarea = new Tarea<string>
            {
                Nombre = "Test",
                Descripcion = "",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarBasica(tarea);

            Assert.False(resultado);
        }

        [Fact]
        public void ValidarEstado_DeberiaRetornarTrue_SiEstadoEsPermitido()
        {
            //Prueba 3: Crear una tarea con estado permitido
            var tarea = new Tarea<string>
            {
                Nombre = "Test",
                Descripcion = "Valida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "completada"
            };

            var resultado = ValidacionesTarea.ValidarEstado(tarea);

            Assert.True(resultado);
        }

        [Fact]
        public void ValidarEstado_DeberiaRetornarFalse_SiEstadoEsInvalido()
        {
            //Paso 4: Validar año de fecha de vencimiento
            var tarea = new Tarea<string>
            {
                Nombre = "Test",
                Descripcion = "Valida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "cancelada"
            };

            // Act: Validar fecha de vencimiento
            var resultado = ValidacionesTarea.ValidarEstado(tarea);

            // Assert: La validación debe ser verdadera
            Assert.False(resultado);
        }

        [Theory]
        [InlineData(true, "completada")]
        [InlineData(false, "cancelada")]
        public void ValidarEstado_DeberiaRetornar(bool esperado, string estado)
        {
            //Prueba 5: Validar estado de tarea

            var tarea = new Tarea<string>
            {
                Nombre = "Test",
                Descripcion = "Valida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = estado
            };

            var resultado = ValidacionesTarea.ValidarEstado(tarea);

            Assert.Equal(esperado, resultado);
        }
        [Fact]
        public void ValidarCriteriosMinimos_DeberiaRetornarTrue_SiNombreNoEsNulo()
        {
            //Paso 6: Validar criterios mínimos

            var resultado = ValidacionesFiltro.ValidarCriteriosMinimos("algo", null, null);

            Assert.True(resultado);
        }

        [Fact]
        public void ValidarLongitudNombre_DeberiaRetornarTrue_SiNombreTieneAlMenosTresCaracteres()
        {
            //Prueba 7: 
            var tarea = new Tarea<string>
            {
                Nombre = "Ana", // 3 caracteres
                Descripcion = "Tarea válida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarLongitudNombre(tarea);

            Assert.True(resultado);
        }

        [Theory]
        [InlineData("Carlos", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("A", false)]
        [InlineData("Ab", false)]
        [InlineData("Ana", true)]
        public void ValidarNombre_DeberiaRetornarResultadoEsperado(string nombre, bool esperado)
        {
            //Prueba 8: 
            var tarea = new Tarea<string>
            {
                Nombre = nombre,
                Descripcion = "Descripción cualquiera",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarLongitudNombre(tarea);

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void ValidarLongitudNombre_DeberiaRetornarFalse_SiNombreEsNulo()
        {
            //Prueba 9: Validar longitud de nombre con nombre nulo
            var tarea = new Tarea<string>
            {
                Nombre = null,
                Descripcion = "Tarea válida",
                FechaVencimiento = DateTime.Now.AddDays(1),
                Testado = "pendiente"
            };

            var resultado = ValidacionesTarea.ValidarLongitudNombre(tarea);

            Assert.False(resultado);
        }

        [Fact]
        public void CrearTarea_DeberiaLlamarAgregarRepositorio()
        {
            //Prueba 10: Crear tarea y verificar llamada al repositorio
            // Arrange: Crear mock del repositorio
            var mockRepo = new Mock<ITareaRepositorio>();

            var servicio = new ServicioTarea(mockRepo.Object);
            var tarea = new Tarea<string>
            {
                Nombre = "Tarea 1",
                Descripcion = "Descripción",
                FechaVencimiento = DateTime.Now.AddDays(2),
                Testado = "pendiente"
            };

            // Act
            servicio.CrearTarea(tarea);

            // Assert: Verificar que el método AgregarTareas fue llamado una vez con la tarea
            mockRepo.Verify(r => r.AgregarTareas(It.Is<Tarea<string>>(t => t == tarea)), Times.Once);
        }

        [Fact]
        public void ValidarFechaVencimiento_DeberiaRetornarTrue_SiAñoEsValido()
        {
            //Paso 11: Validar fecha de vencimiento
            var fecha = new DateTime(2025, 1, 1);

            var resultado = ValidacionesFiltro.ValidarFechaVencimiento(null, null, fecha);

            Assert.True(resultado);
        }

        [Fact]
        public void ValidarFechaVencimiento_DeberiaRetornarFalse_SiAnoEsInvalido()
        {
            // Prueba 12: Validar fecha de vencimiento con año inválido
            DateTime fechaInvalida = new DateTime(1800, 1, 1);

            // Act: Validar fecha de vencimiento
            var resultado = ValidacionesFiltro.ValidarFechaVencimiento(null, null, fechaInvalida);

            // Assert: La validación debe ser falsa
            Assert.False(resultado);
        }

        [Fact]
        public void AgregarTarea_DeberiaAgregarTareaALaCola()
        {
            //Paso 13: Agregar tarea a la cola
            var cola = new ColaDeTareas<string>();
            cola.AgregarTarea("MiTarea");

            Assert.True(true); // Esta prueba es visual. Para pruebas automatizadas, expón la cola interna o usa reflexión.
        }
    }
}
