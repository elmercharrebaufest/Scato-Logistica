using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Precinto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }
        [Required]
        public virtual string NumeroPrecinto { get; set; }
        public virtual string Detalle { get; set; }
    }
}
