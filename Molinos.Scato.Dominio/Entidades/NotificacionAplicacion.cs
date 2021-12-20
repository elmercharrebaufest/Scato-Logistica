using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("NotificacionAplicacion")]
    public class NotificacionAplicacion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string TipoAccion { get; set; }
        public virtual DateTime FechaAccion { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string Detalle { get; set; }
    }
}
