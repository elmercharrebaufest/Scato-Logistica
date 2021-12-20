using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class FinalizarRomaneo : Comando
    {
        public Guid WorkflowId { get; set; }
    }
}