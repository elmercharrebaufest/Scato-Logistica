using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpIdentificacionMuestraCalado")]
    public class ImpIdentificacionMuestraCalado : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string NumeroCartaPorte { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string Humedad { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual DateTime FechaCalado { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual string NumeroDeOrden { get; set; }
    }
}
