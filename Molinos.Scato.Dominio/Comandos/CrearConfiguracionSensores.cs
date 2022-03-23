using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearConfiguracionSensores : Comando
    {
        public ConfigSensoresDto Dto { get; set; }
    }
}
