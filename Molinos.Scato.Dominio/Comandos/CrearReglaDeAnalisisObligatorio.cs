using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearReglaDeAnalisisObligatorio : Comando
    {
        public ReglaDeAnalisisObligatorioDto Dto { get; set; }
    }
}
