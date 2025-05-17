namespace StrategyPattern
{
    public class EstrategiaDelBorrachoContexto
    {
        private IBorracho _borracho;

        public enum Comportamiento
        {
            HacerOjitos,
            InvitarCerveza,
            HacerCaraDeGalan
        }

        public EstrategiaDelBorrachoContexto()
        {
            this._borracho = new EstrategiaOjitos(); // Estrategia por defecto
        }

        public void Conquistar(Comportamiento opcion)
        {
            switch (opcion)
            {
                case Comportamiento.HacerOjitos:
                    this._borracho = new EstrategiaOjitos();
                    break;
                case Comportamiento.InvitarCerveza:
                    this._borracho = new EstrategiaInvitarCerveza();
                    break;
                case Comportamiento.HacerCaraDeGalan:
                    this._borracho = new EstrategiaCaraDeGalan();
                    break;
                default:
                    Console.WriteLine("Comportamiento no reconocido.");
                    return;
            }

            this._borracho.Conquistar(); // Ejecutar la estrategia seleccionada
        }
    }
}
