using System;

namespace Molinos.Scato.Servicios
{
    [Serializable]
    public class MercadoLibreException : Exception
    {
        public MercadoLibreException(string message) : base(message)
        {
        }
        
    }
}
