using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCamara : Comando
    {
        public CamaraDto Dto { get; set; }
    }
}
