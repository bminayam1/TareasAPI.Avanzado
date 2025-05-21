
using System;
using FactoryMethod2;

namespace FactoryMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            BebidaEmbriagante bebida = Creador.CrearBebida(Creador.CERVEZA);
            Console.WriteLine($"Nivel de embriaguez por hora: {bebida.CuantoMeEmbriagaPorHora()}");
        }
    }
}
