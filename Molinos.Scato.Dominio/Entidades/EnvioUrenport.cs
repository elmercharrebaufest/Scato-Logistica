using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EnvioUrenport : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual string TipoDoc { get; set; }
        public virtual string Ruta { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual EstadoTransmisionASap Estado { get; set; }
        public virtual string Error { get; set; }
        public virtual DateTime Fecha { get; set; }
    }
}
