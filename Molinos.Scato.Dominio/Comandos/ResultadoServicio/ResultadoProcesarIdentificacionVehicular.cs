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
        public LecturaPuestoDeTrabajoDto LecturaPuestoDeTrabajo { get; set; }
    }
}
