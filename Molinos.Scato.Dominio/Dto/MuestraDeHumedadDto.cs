using System;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraDeHumedadDto
    {
        [DataMember]
        public int Id { get; set; }

        public Guid WorkflowInstanceId { get; set; }

        public string Centro { get; set; }

        public string Humedimetro { get; set; }


        public string Modalidad { get; set; }

        public string NumeroOrden { get; set; }

        public string NumeroDocumentoIngreso { get; set; }

        public int CicloDeCalado { get; set; }

        public int NroDeToma { get; set; }

        public decimal ValorLeido { get; set; }

        public decimal ValorFinal { get; set; }

        public DateTime Fecha { get; set; }

        public string Usuario { get; set; }
        
        public int CentroId { get; set; }

        public int HumedimetroId { get; set; }
        public int? MotivoHumedadManualId { get; set; }

        public string MotivoHumedadManualDesc { get; set; }

        public string Material { get; set; }

        public string Dispositivo { get; set; }
    }
}
