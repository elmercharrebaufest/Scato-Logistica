using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarCartaPorteUnreport : Comando
    {
        public int CartaPorteId { get; set; }
        public Guid WorkflowId { get; set; }

        public int EnvioUrenport { get; set; }
    }
}
