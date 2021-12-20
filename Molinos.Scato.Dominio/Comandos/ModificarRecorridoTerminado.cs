using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoTerminado : Comando
    {
        public Guid InstanceId { get; set; }
    }
}