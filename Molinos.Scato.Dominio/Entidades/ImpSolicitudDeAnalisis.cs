using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpSolicitudDeAnalisis")]
    public class ImpSolicitudDeAnalisis : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string NumeroAnalisis { get; set; }
        public virtual string Material { get; set; }
        public virtual string CaracteristicaDeCalidad { get; set; }
        public virtual DateTime FechaCalado { get; set; }
        public virtual string NumeroDeOrden { get; set; }
    }
}
