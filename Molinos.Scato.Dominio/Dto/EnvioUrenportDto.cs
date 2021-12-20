using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{

    public sealed class EnvioUrenportDto
    {
        public int Id { get; set; }
        public int RecorridoId { get; set; }
        public EstadoTransmisionASap Estado { get; set; }
        public DateTime Fecha { get; set; }
        public string Error { get; set; }
        public string TipoDoc { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Ruta { get; set; }
        public string Patente { get; set; }
    }
}