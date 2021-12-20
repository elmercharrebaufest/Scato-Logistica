using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearImpresora : Comando
    {
        public ImpresoraDto Dto { get; set; }
    }
}
