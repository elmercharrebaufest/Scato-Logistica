using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearDocumentoDeImpresionPorCentro : Comando
    {
        public DocumentoDeImpresionPorCentroDto Dto { get; set; }
    }
}
