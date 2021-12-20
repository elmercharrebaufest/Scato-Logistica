using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMotivoQuiebreBarrera : Comando
    {
        public MotivoQuiebreBarreraDto Dto { get; set; }
    }
}
