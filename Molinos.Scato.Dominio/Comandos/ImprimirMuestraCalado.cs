
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirMuestraCalado : Comando
    {
        public ImpIdentificacionMuestraCaladoDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
