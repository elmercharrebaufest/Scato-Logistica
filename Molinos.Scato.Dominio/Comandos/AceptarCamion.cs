using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AceptarCamion : Comando
    {
        public Guid WorkflowId { get; set; }
    }
}
