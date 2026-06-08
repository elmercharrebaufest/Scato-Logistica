using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoProcesarIdentificacionVehicular : Resultado
    {
        [DataMember]
        public int LogId { get; set; }

        [DataMember]
        public bool AvanzarWorkflow { get; set; }

        [DataMember]
        public int? RecorridoId { get; set; }

        [DataMember]
        public int? PuestoDeTrabajoId { get; set; }

        [DataMember]
        public string ResultadoWorkflow { get; set; }

        [DataMember]
        public LecturaPuestoDeTrabajoDto LecturaPuestoDeTrabajo { get; set; }
    }
}
