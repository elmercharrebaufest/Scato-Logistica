using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearBocaDestino : Comando
    {
        public BocaDestinoDto Dto { get; set; }
    }
}
