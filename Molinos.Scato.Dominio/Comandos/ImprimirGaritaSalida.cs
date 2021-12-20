
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirGaritaSalida : Comando
    {
        public ImpGaritaSalidaDto Dto { get; set; }
        public int CantidadCopias { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
