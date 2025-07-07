
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirReciboMunicipal : Comando
    {
        public ImpReciboMunicipalDto Dto { get; set; }
        public int CantidadCopias { get; set; }
        public FirmaDto Firma { get; set; }
        public int IdPagoDigital { get; set; }
    }
}
