using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PagosTasaMunicipal : IIdentificable
    {
        [Key]
        public int Id { get; set; }

        [Column("MOAPay_Id")]
        public int IdMOAPay { get; set; }

        [StringLength(20)]
        public string NumeroDocumento { get; set; }

        [StringLength(20)]
        public string CuitInterviniente { get; set; }

        [StringLength(20)]
        public string Dominio { get; set; }

        [StringLength(10)]
        public string TipoVehiculo { get; set; }

        public DateTime? FechaPago { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal? Importe { get; set; }
        public DateTime? FechaAcceso { get; set; }

        [Column("Instance_Id")]
        public virtual Guid? IdInstance { get; set; }
        public virtual string TipoDocumento { get; set; }
        public virtual bool Disponible { get; set; }
        public virtual string NroRecibo { get; set; }
    }
}