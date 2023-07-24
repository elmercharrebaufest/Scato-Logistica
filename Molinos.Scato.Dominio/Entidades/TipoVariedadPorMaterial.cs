using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoVariedadPorMaterial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual TipoVariedad TipoVariedad { get; set; }
        public virtual Material Material { get; set; }

    }
}
