using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearTipoDocumentoIdentidad : Comando
    {
        public TipoDocumentoIdentidadDto Dto { get; set; }
    }
}
