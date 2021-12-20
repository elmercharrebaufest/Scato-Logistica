using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CiuAnulado : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual DateTime Fecha { get; set; }
    }
}
