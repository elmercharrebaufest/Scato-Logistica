using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("InformarCupoTransmisionASap")]
    public class InformarCupoTransmisionASap : TransmisionASap
    {
        public virtual string NumeroCartaPorte { get; set; }
        public virtual string CodigoCupo { get; set; }
        public virtual string FechaIngreso { get; set; }
        public virtual string HoraIngreso { get; set; }
        public virtual string TitularCpCodigoSap { get; set; }
        public virtual string TitularCpDescripcion { get; set; }
        public virtual string RtteComercialCodigoSap { get; set; }
        public virtual string RtteComercialDescripcion { get; set; }
        public virtual string CorredorCodigoSap { get; set; }
        public virtual string CorredorDescripcion { get; set; }
        public virtual string AgenteDeComprasCodigoSap { get; set; }
        public virtual string AgenteDeComprasDescripcion { get; set; }
        public virtual string DestinatarioCodigoSap { get; set; }
        public virtual string DestinatarioDescripcion { get; set; }
        public virtual string EstablecimientoCodigo { get; set; }
        public virtual string CentroId { get; set; }
        public virtual string CentroDescripcion { get; set; }
        public virtual string MaterialCodigoSap { get; set; }
        public virtual string MaterialDescripcion { get; set; }
        public virtual string FechaTara { get; set; }
        public virtual string HoraTara { get; set; }
        public virtual string FechaEgreso { get; set; }
        public virtual string HoraEgreso { get; set; }
        public virtual string Rechazado { get; set; }

        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}
