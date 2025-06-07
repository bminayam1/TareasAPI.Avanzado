using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsignacionFuncionesExtensiones
{
    public static class Extensiones
    {
        // Extensión para capitalizar cada palabra en un string
        public static string CapitalizarPalabras(this string texto)
        {
            return string.Join(" ", texto.Split(' ')
                                         .Select(palabra => char.ToUpper(palabra[0]) + palabra.Substring(1).ToLower()));
        }

        // Extensión para calcular la mediana de una lista de enteros
        public static double Mediana(this List<int> numeros)
        {
            if (numeros == null || numeros.Count == 0) return 0.0;

            var ordenados = numeros.OrderBy(n => n).ToList();
            int mitad = ordenados.Count / 2;

            return ordenados.Count % 2 != 0
                ? ordenados[mitad]
                : (ordenados[mitad - 1] + ordenados[mitad]) / 2.0;
        }

        // Extensión para verificar si una fecha es fin de semana
        public static bool EsFinDeSemana(this DateTime fecha)
        {
            return fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
