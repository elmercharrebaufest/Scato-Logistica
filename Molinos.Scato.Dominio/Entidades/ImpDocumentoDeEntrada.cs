using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpDocumentoDeEntrada")]
    public class ImpDocumentoDeEntrada : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroDeIngreso { get; set; }
        public virtual DateTime FechaDocumentoDeIngreso { get; set; }
    }
}
