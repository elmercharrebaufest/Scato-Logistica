using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MovimientoDeTercerosListaDto
    {
        private string nroCartaPorte;
        public int Id { get; set; }
        public DateTime FechaDescarga { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public string NroCartaPorte
        {
            get { return (nroCartaPorte ?? "0").Replace("-", "").Replace("R", "").PadLeft(12, '0'); }
            set { nroCartaPorte = value; }
        }
        public string NroCartaPorteConFormato
        {
            get { return (nroCartaPorte ?? "0"); }
        }
        public string Patente { get; set; }
        public string Material { get; set; }      
        public int? PesoNeto { get; set; }
        public string Transportista { get; set; }
        public string CentroDestino { get; set; }
    }
}
