using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCalado : Comando
    {
        public CaladoDto Dto { get; set; }
        public CaladoPorCaracteristicaDto[] CaladosPorCaracteristica { get; set; }
        public int PesoNetoOrigen { get; set; }
    }
}
