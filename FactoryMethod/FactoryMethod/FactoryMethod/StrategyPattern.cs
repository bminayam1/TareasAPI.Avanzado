using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            EstrategiaDelBorrachoContexto estrategia = new EstrategiaDelBorrachoContexto();
            _Borracho.Conquistar(EstrategiaDelBorrachoContexto.Comportamiento.HacerOjitos);
            _Borracho.Conquistar(EstrategiaDelBorrachoContexto.Comportamiento.InvitarCerveza);
        }

    }
}
