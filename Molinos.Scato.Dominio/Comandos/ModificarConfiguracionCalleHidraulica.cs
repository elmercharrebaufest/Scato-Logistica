using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarConfiguracionCalleHidraulica : Comando
    {
        public ConfiguracionCalleHidraulicaDto Dto { get; set; }
    }
}
