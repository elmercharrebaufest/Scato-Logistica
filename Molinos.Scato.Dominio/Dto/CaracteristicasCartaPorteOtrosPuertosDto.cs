using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CaracteristicasCartaPorteOtrosPuertosDto
    {
  
        public int Id { get; set; }
        public int CartaPorteOtrosPuertosId { get; set; }
        public string CodigoExterno { get; set; }
        public decimal Valor { get; set; }
    }
}
