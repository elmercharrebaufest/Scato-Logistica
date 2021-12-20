using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Observacion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public Guid WorkflowInstanceId { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
