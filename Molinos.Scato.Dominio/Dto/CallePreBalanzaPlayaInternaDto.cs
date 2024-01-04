using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class CallePreBalanzaPlayaInternaDto
    {
        public int Id { get; set; }
        public CalleDto CallePlayaInterna { get; set; }
        public CalleDto CallePreBalanza { get; set; }
        public DateTime FechaLlamado { get; set; }
        public int? RecorridoId { get; set; }
        public string CodigoAutomatismoTipoLlamado { get; set; }
        public bool EsCamionEnEspera { get; set; }
    }
}