
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirAsignacionDeRuta : Comando
    {
        public ImpAsignacionDeRutaDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
