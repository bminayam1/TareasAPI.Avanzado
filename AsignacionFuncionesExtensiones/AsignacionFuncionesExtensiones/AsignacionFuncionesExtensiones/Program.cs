using System;
using System.Collections.Generic;
using AsignacionFuncionesExtensiones;

class Program
{
    static void Main()
    {
        Console.WriteLine("🔹 Prueba de funciones puras:");
        Console.WriteLine($"Suma de dígitos (123): {FuncionesPuras.SumarDigitos(123)}");
        Console.WriteLine($"¿Es palíndromo? (121): {FuncionesPuras.EsPalindromo(121)}");
        Console.WriteLine($"Promedio de lista [4, 8, 15, 16, 23]: {FuncionesPuras.CalcularPromedio(new List<int> { 4, 8, 15, 16, 23 })}");

        Console.WriteLine("\n🔹 Prueba de extensiones:");
        Console.WriteLine($"Capitalizar palabras ('hola mundo'): {"hola mundo".CapitalizarPalabras()}");
        Console.WriteLine($"Mediana de [3, 1, 4, 2]: {new List<int> { 3, 1, 4, 2 }.Mediana()}");
        Console.WriteLine($"¿Es fin de semana? (12/02/2024): {new DateTime(2024, 2, 12).EsFinDeSemana()}");
    }
}
