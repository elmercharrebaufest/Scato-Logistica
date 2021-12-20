using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PinchazosPorCalada : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual TipoPinchazo TipoPinchazo { get; set; }
        [Required]
        public DateTime FechaModificacion { get; set; }
        [Required]
        public int Usuario_Id { get; set; }
        public int Centro_Id { get; set; }
        public string Motivo { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
        [ForeignKey("Centro_Id")]
        public virtual Centro Centro { get; set; }

    }
}
