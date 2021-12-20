using Molinos.Scato.Servicios.Conversiones.Impl;

namespace Molinos.Scato.Test
{
    public class FactoryConversor
    {
        private static readonly ConversorAutoMapper conversor = new ConversorAutoMapper();

        public static ConversorAutoMapper ConversorAutoMapper
        {
            get { return conversor; }
        }
    }
}
