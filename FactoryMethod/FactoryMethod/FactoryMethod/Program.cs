using System;
using StrategyPattern;

namespace StrategyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            EstrategiaDelBorrachoContexto borracho = new EstrategiaDelBorrachoContexto();

            borracho.Conquistar(EstrategiaDelBorrachoContexto.Comportamiento.HacerOjitos);
            borracho.Conquistar(EstrategiaDelBorrachoContexto.Comportamiento.InvitarCerveza);
            borracho.Conquistar(EstrategiaDelBorrachoContexto.Comportamiento.HacerCaraDeGalan);
        }
    }
}
