using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Notificacion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public string Grupo { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual DateTime Hora { get; set; }
        public virtual bool Leido { get; set; }
        public virtual TipoAlerta TipoAlerta { get; set; }
        public virtual int? PuestoId { get; set; }
    }
}
