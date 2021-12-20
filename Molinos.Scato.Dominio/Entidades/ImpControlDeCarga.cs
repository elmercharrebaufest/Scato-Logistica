using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpControlDeCarga")]
    public class ImpControlDeCarga : Impresion
    {
        public virtual string NumeroControl { get; set; }
        public virtual string Centro { get; set; }
        public virtual DateTime FechaDocumentoDeIngreso { get; set; }
        public virtual decimal? PesoBruto { get; set; }
        public virtual decimal? PesoTara { get; set; }
        public virtual decimal? PesoNeto { get; set; }
        public virtual decimal? TotalDescargado { get; set; }
        public virtual decimal? Diferencia { get; set; }
    }
}
