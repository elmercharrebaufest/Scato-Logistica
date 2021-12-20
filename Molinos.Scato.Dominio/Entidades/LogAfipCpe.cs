using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogAfipCpe : IIdentificable
    {
        public virtual int Id { get; set; }
        public string Servicio { get; set; }
        public string Consulta { get; set; }
        public string Respuesta { get; set; }
        public DateTime Fecha { get; set; }
    }
}
