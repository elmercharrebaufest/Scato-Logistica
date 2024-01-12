using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarAsignacionNoGranoEnRecorrido : Comando
    {
        public AsignacionNoGranoEnRecorridoDto Dto { get; set; }
    }
}
