using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VariedadPorMaterial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual VariedadMaterial VariedadMaterial { get; set; }
        public virtual Material Material { get; set; }

    }
}
