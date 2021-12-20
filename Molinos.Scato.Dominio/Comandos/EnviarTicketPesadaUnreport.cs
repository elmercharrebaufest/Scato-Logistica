using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarTicketPesadaUnreport : Comando
    {
        public int TicketPesadaId { get; set; }
        public Guid WorkflowId { get; set; }
        public int EnvioUrenport { get; set; }
    }
}
