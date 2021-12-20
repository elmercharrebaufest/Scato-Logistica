using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMaterialPuerto : Comando
    {
        public MaterialPuertoDto Dto { get; set; }
    }
}
