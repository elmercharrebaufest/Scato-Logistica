using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class FinDeWorkflow : Comando
    {
        public Guid InstanceId { get; set; }
    }
}
