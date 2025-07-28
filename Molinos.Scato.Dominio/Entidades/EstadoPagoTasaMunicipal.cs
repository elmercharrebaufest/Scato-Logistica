using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EstadoPagoTasaMunicipal 
    {
        [Key]
        public virtual int IdPagoNormal { get; set; }
        public virtual int? IdDiferenciaPago { get; set; }
        public virtual int IdPayPagoNormal { get; set; }
        public virtual int? IdPayDiferenciaPago { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string Dominio { get; set; }
        public virtual DateTime FechaPago { get; set; }
        public virtual decimal ImporteNormal { get; set; }
        public virtual decimal ImporteDP { get; set; }
        public virtual decimal TotalPagado { get; set; }
        public virtual decimal TarifaTipoVehiculo { get; set; }
        public virtual TipoValidacionPagoTasaMunicipal CondicionDePago { get; set; }
    }
}