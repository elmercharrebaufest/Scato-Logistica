using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarConfiguracionDeTabla : Comando
    {
        public ConfiguracionDeTablaDto Dto { get; set; }
    }
}
