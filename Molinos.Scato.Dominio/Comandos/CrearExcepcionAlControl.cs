using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearExcepcionAlControl : Comando
    {
        public ExcepcionAlControlDto Dto { get; set; }
    }
}
