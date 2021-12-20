using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMotivo : Comando
    {
        public MotivoDto Dto { get; set; }
    }
}
