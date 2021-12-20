using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirResumenHojaDeRuta : Comando
    {
        public ImpResumenHojaDeRutaDto Dto { get; set; }
    }
}
