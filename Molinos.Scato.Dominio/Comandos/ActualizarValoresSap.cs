using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarValoresSap : Comando
    {
        public string NumeroDeDocumento { get; set; }
        public Guid InstanceId { get; set; }
        public int CentroId { get; set; }
        public int WorkflowDefinicionId { get; set; }
    }
}
