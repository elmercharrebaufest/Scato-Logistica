using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarRegistroJobEjecucion : Comando
    {
        public RegistroJobEjecucionDto Dto { get; set; }
    }
}