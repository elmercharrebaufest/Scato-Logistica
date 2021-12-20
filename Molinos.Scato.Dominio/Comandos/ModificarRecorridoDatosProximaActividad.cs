using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoDatosProximaActividad : Comando
    {
        public Guid InstanceId { get; set; }
        public string DatosProximaActividad { get; set; }
    }
}
