using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarInhabilitacionCamion : Comando
    {
        public InhabilitacionCamionDto Dto { get; set; }
    }
}
