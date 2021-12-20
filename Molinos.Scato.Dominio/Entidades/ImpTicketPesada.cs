using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpTicketPesada")]
    public class ImpTicketPesada : Impresion
    {
        public virtual string NumeroIngreso { get; set; }
        public virtual string Emisor { get; set; }
        public virtual string DoimicilioCentro { get; set; }
        public virtual string TipoDocumento { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string Material { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string Remitente { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string CuitTransportista { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc
    }
}
