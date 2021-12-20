using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LogCambioDeModalidadCalleDto
    {
        public int Id { get; set; }
        public bool Activado{ get; set; }
        public string Motivo{ get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public int CalleId  { get; set; }
    }
}
