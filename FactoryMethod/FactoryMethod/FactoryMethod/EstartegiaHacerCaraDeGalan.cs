using StrategyPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern
{
    public class EstrategiaCaraDeGalan : IBorracho
    {
        public void Conquistar()
        {
            Console.WriteLine("Hacer cara de galán a la muchacha.");
        }
    }
}
