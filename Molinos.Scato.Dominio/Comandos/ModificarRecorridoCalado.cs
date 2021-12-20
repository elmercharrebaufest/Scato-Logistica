using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoCalado : Comando
    {
        public Guid WorkflowInstanceId { get; set; }
    }
}