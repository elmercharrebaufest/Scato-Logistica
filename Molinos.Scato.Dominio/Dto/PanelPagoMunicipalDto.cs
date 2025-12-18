using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class PanelPagoMunicipalDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NumeroDocumento { get; set; }
        public string WorkflowCodigo { get; set; }
        public string WorkflowDescripcion { get; set; }
        public string CuitInterviniente { get; set; }
        public string Patente { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public decimal? Importe { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string NumeroRecibo { get; set; }
        public bool PagoInformado { get; set; }
    }
}
