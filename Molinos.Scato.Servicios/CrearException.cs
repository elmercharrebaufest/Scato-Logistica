using System;

namespace Molinos.Scato.Servicios
{
    [Serializable]
    public class CrearException : Exception
    {
        public CrearException(string message) : base(message)
        {
        }
        
    }
}
