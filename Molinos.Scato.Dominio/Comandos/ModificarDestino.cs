using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarDestino : Comando
    {
        public DestinoDto Dto { get; set; }
    }
}
