using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCamara : Comando
    {
        public CamaraDto Dto { get; set; }
    }
}
