using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ControlDeBalanzaComando : Comando
    {
        public ControlDeBalanzaDto Dto { get; set; }
        public bool Finalizar { get; set; }
    }
}
