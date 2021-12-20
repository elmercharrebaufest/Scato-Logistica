
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirEnvioLoteACamara : Comando
    {
        public ImpIdentificacionEnvioLoteACamaraDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
