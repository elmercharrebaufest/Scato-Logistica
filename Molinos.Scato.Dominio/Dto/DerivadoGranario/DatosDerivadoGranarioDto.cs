using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class DatosDerivadoGranarioDto
    {
        public virtual string NroCTG { get; set; }
        public virtual string Sucursal { get; set; }
        public virtual string NroOrden { get; set; }
    }
}
