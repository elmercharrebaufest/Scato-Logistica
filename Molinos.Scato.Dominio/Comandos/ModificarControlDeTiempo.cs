using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarControlDeTiempo : Comando
    {
        public ControlDeTiempoDto Dto { get; set; }
    }
}
