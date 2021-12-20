using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ControlDeBalanzaPesadaDto
    {
        public int Id { get; set; }
        public int ControlDeBalanzaId { get; set; }
        public int BalanzaId { get; set; }
        public string BalanzaNombre { get; set; }
        public int Peso { get; set; }
        public DateTime Fecha { get; set; }
        public string Patente { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public TipoPesada TipoPesada { get; set; }
        public string Observacion { get; set; } //TODOSACAR
    }
}