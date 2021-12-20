using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarCaracteristicaDeCalidad : Comando
    {
        public CaracteristicaDeCalidadDto Dto { get; set; }
        public List<int> DescuentosBorrados { get; set; }
    }
}
