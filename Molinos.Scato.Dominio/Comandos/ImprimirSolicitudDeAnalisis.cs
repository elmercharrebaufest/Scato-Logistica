
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirSolicitudDeAnalisis : Comando
    {
        public ImpSolicitudDeAnalisisDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
