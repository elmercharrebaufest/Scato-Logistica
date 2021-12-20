using System;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCrearWorkflow : ResultadoCrear
    {
        [DataMember]
        public Guid InstanciaWorkflowId { get; set; }
    }
}
