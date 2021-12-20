using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearNirs : Comando
    {
        public NirsDto Dto { get; set; }
    }
}
