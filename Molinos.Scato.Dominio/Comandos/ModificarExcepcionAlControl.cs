using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarExcepcionAlControl : Comando
    {
        public ExcepcionAlControlDto Dto { get; set; }
    }
}
