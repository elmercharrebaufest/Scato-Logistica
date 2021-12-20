using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearAnalisisObligatorio : Comando
    {
        public AnalisisObligatorioDto Dto { get; set; }
    }
}
