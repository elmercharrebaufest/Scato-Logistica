using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearConversionCaracteristica : Comando
    {
        public ConversionCaracteristicaDto Dto { get; set; }
    }
}
