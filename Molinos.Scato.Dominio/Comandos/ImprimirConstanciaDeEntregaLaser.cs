
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirConstanciaDeEntregaLaser : Comando
    {
        public ImpConstanciaDeEntregaLaserDto Dto { get; set; }
        public int CantidadCopias { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
