using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MotivoForzarCero : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Balanza Balanza { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Motivo { get; set; }
        public virtual Guid InstanceId { get; set; }
        public virtual string Usuario { get; set; }
    }
}
