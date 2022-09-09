using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class LogDispositivoDto
    {
        public int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public string CodigoDispositivo { get; set; }
        public string NombreLog { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
    }
}