using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarVapor : Comando
    {
        public VaporDto Dto { get; set; }
    }
}
