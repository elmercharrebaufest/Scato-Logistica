using System;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraDeInaseDto
    {
        [DataMember]
        public int Id { get; set; }
        public int RecorridoId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public int CentroId { get; set; }
        public DateTime Fecha { get; set; }
        public string CartaPorte { get; set; }
    }
}
