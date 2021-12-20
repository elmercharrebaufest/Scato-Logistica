using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpTicketPesadaBodegaDto
    {
        public int Id { get; set; }
        public DateTime? FechaUltimaPesada { get; set; }
        public string ComprobanteInterno { get; set; }
        public string Material { get; set; }
        public string Proveedor { get; set; }
        public string Remito { get; set; }
        public string PesoBrutoBodega { get; set; }
        public string PesoTaraBodega { get; set; }
        public string BalanzaBruto { get; set; }
        public string BalanzaTara { get; set; }
        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public string CIU { get; set; }
        public string Pedido { get; set; }
        public IList<CaladoPorCaracteristicaDto> RubrosCalados { get; set; }
        public IdentidadDeCopia Identidad { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public virtual string Codigo { get; set; }
        public string Impresora { get; set; }
        public string CentroDescripcion { get; set; }
        public string CentroLocalidad { get; set; }
        public string CentroDireccion { get; set; } 


    }
}
