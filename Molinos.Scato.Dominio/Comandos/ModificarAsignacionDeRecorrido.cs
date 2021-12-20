using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAsignacionDeRecorrido : Comando
    {
        public AsignacionDeRecorridoDto Dto { get; set; }
    }
}
