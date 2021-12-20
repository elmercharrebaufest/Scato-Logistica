using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearTransaccionSAP : Comando
    {
        public TransaccionSAPDto Dto { get; set; }
    }
}
