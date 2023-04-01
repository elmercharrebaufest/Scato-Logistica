using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearClienteProvisorio : Comando
    {
        public ClienteDto Dto { get; set; }
    }
}
