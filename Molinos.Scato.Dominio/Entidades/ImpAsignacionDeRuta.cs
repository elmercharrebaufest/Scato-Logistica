using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpAsignacionDeRuta")]
    public class ImpAsignacionDeRuta : Impresion
    {
        public virtual string NumeroDeOrden { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Material { get; set; }
        public virtual string MaterialCodigoSap { get; set; }
        public virtual string BalanzaBruto { get; set; }
        public virtual string Calle { get; set; }
        public virtual string Hidraulicas { get; set; }
        public virtual string Almacen { get; set; }
        public virtual string BalanzaTara { get; set; }
        public virtual string Humedad { get; set; }
        public virtual string Calidad { get; set; }
        public virtual DateTime? FechaCalado { get; set; }
        public virtual string Observacion { get; set; }
        public virtual string TipoVehiculo { get; set; }

    }
}
