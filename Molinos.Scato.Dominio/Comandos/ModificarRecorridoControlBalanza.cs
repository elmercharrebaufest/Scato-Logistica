using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoControlBalanza : Comando
    {
        public Guid InstanceId { get; set; }
        public bool ControlBalanza { get; set; }
    }
}
