using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearConversionMaterial : Comando
    {
        public ConversionMaterialDto Dto { get; set; }
    }
}
