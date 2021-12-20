using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class LlegadaADestinoEnRedespachosTransmisionASapDto : TransmisionASapDto
    {
        public Guid WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
    }
}