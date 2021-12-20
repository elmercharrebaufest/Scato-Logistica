using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EquivalenciasCaracteristicasCPOtrosPuertosDto
    {
        public int Id { get; set; }        
        public int MaterialId { get; set; }
        public string CodigoExterno { get; set; }
        public string CodigoSap { get; set; }
    }
}
