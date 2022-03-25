using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearConfiguracionGeneral : Comando
    {
        public ConfiguracionGeneralDto Dto { get; set; }
    }
}