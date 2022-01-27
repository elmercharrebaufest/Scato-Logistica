using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MotivoInactividad:IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        [Display(Name = "Motivo de Inactividad")]
        public virtual string Descripcion { get; set; }
    }
}
