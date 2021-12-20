using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class RechazarCamion : Comando
    {
        public Guid WorkflowId { get; set; }
    }
}
