using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpReciboMunicipalImportacion")]
    public class ImpReciboMunicipalImportacion : Impresion
    {
        public virtual string Ordenanza { get; set; }
        public virtual string Valor { get; set; }
        public virtual string NroDocumentoLegal { get; set; }
        public virtual string TicketNro { get; set; }
    }
}
