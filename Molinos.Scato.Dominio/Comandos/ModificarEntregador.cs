using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEntregador : Comando
    {
        public EntregadorDto Dto { get; set; }
    }
}
