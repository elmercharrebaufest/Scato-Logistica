using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarAnalisisObligatorio : Comando
    {
        public AnalisisObligatorioDto Dto { get; set; }
    }
}
