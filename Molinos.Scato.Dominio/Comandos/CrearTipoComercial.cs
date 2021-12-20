using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearTipoComercial : Comando
    {
        public TipoComercialDto Dto { get; set; }
    }
}
