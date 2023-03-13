using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarVisualizacionBarrera : Comando
    {
        public VisualizacionBarreraDto Dto { get; set; }
        public List<int> SensoresBorrados { get; set; }
    }
}
