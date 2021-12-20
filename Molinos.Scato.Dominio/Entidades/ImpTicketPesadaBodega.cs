using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpTicketPesadaBodega")]
    public class ImpTicketPesadaBodega : Impresion
    {
        public virtual DateTime? FechaUltimaPesada { get; set; }
        public virtual string ComprobanteInterno { get; set; }
        public virtual string Material { get; set; }
        public virtual string Proveedor { get; set; }
        public virtual string Remito { get; set; }
        public virtual string PesoBrutoBodega { get; set; }
        public virtual string PesoTaraBodega { get; set; }
        public virtual string BalanzaBruto { get; set; }
        public virtual string BalanzaTara { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string Chofer { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual DateTime FechaIngreso { get; set; }
        public virtual DateTime? FechaEgreso { get; set; }
        public virtual string CIU { get; set; }
        public virtual string Pedido { get; set; }
        public virtual string CentroDescripcion { get; set; }
        public virtual string CentroLocalidad { get; set; }
        public virtual string CentroDireccion { get; set; }     
        [InverseProperty("ImpTicketPesadaBodega")]
        public virtual ICollection<CaladoPorCaracteristica> RubrosCalados { get; set; }
        public virtual IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc
    }
}
