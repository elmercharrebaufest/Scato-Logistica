using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VisecTransmisionMovimiento : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string NumeroRENSPA { get; set; }
        public virtual string NumeroCTGAsignado { get; set; }
        public virtual int? PesoNetoCargaKgPorUP { get; set; }
        public virtual int? PesoNetoDescargaKgPorUP { get; set; }
        public virtual int? PesoIngresoStockKg { get; set; }
        public virtual string UltimoAlmacenamiento { get; set; }
        public virtual int TipoMovimiento { get; set; }

        [Column("VisecTransmision_Id")]
        public virtual int VisecTransmisionId { get; set; }
        public virtual VisecTransmision VisecTransmision { get; set; }
    }
}