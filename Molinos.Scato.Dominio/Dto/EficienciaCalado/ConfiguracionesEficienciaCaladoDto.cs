using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class ConfiguracionesEficienciaCaladoDto
    {
        public ConfiguracionGeneralDto EficienciaCalle { get; set; }
        public ConfiguracionGeneralDto Horario { get; set; }
    }
}
