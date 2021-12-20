using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarTipoDocumentoIdentidad : Comando
    {
        public TipoDocumentoIdentidadDto Dto { get; set; }
    }
}
