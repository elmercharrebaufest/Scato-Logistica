using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class PinchazosPorCaladaDto
    {
        public virtual TipoPinchazo TipoPinchazo { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string Motivo { get; set; }

        public virtual UsuarioDto Usuario { get; set; }
    }
}
