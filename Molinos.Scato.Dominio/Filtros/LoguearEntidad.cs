using System;

namespace Molinos.Scato.Dominio.Filtros
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoguearEntidad : Attribute
    {
        public bool Loguear { get; set; }
        public LoguearEntidad(bool loguear = true)
        {
            this.Loguear = loguear;
        }
    }
}
