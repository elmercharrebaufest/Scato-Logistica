using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpReciboMunicipal")]
    public class ImpReciboMunicipal : Impresion
    {
        public virtual string Ordenanza { get; set; }
        public virtual string Valor { get; set; }
        public virtual string NroDocumentoLegal { get; set; }
        public virtual string TicketNro { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }
    }
}
