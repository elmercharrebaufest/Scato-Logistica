using System.Collections.Generic;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoProcesarIdentificacionVehicular : Resultado
    {
        [DataMember]
        public LecturaPuestoDeTrabajoDto LecturaPuestoDeTrabajo { get; set; }

        [DataMember]
        public bool EnviadoAContingencia { get; set; }

        [DataMember]
        public List<int> PuestosDeTrabajoId { get; set; }
    }
}
