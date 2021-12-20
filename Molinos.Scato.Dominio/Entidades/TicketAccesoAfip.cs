using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TicketAccesoAfip : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Token { get; set; }
        public virtual string Sign { get; set; }
        public virtual DateTime ExpirationTime { get; set; }
        public virtual DateTime GenerationTime { get; set; }
        public virtual string Service { get; set; }
        public virtual string CuitRepresentado { get; set; }
        public virtual DateTime? FechaCreacion { get; set; }
    }
}
