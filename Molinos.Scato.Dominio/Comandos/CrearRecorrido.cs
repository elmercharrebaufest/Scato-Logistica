using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearRecorrido : Comando
    {
        public Guid InstanceId { get; set; }
        public Guid InstanceIdViejo { get; set; }
        public string NombreWorkflow { get; set; }
        public int WorkflowDefinicionId { get; set; }
    }
}
