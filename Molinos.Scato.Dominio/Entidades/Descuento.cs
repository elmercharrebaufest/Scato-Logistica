using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Descuento : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual decimal ValorHasta { get; set; }
        public virtual decimal PorcentajeDescuento { get; set; }
        public virtual decimal PorcentajeEnvioCamaraAuditoria { get; set; }
        public bool MercadoATermino { get; set; }

        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
    }
}
