using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpIdentificacionEnvioLoteACamara")]
    public class ImpIdentificacionEnvioLoteACamara : Impresion
    {
#pragma warning disable 169
        private string precinto;
        public virtual string Centro { get; set; }
        public virtual string NumeroDeMuestra { get; set; }
        public virtual DateTime FechaCalado { get; set; }
        public virtual string NumeroDeOrden { get; set; }
        public virtual string Precinto { get; set; }
#pragma warning restore 169
    }
}
