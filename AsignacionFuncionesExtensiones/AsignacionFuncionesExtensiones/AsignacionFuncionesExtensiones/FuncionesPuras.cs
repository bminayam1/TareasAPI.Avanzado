using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsignacionFuncionesExtensiones
{
    public static class FuncionesPuras
    {
        // Suma de los dígitos de un número entero
        public static int SumarDigitos(int numero)
        {
            return numero.ToString().Select(d => int.Parse(d.ToString())).Sum();
        }

        // Determinación de palíndromos
        public static bool EsPalindromo(int numero)
        {
            string strNumero = numero.ToString();
            return strNumero.SequenceEqual(strNumero.Reverse());
        }

        // Promedio de una lista de números enteros
        public static double CalcularPromedio(List<int> numeros)
        {
            if (numeros == null || numeros.Count == 0) return 0.0;
            return numeros.Average();
        }
    }
}
