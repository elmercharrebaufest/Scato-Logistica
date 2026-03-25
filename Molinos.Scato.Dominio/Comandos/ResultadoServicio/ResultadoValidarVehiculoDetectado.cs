using System.Collections.Generic;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoValidarVehiculoDetectado : Resultado
    {
        [DataMember]
        public List<LecturaPuestoDeTrabajoDto> LecturaPuestosDeTrabajo { get; set; }

        public ResultadoValidarVehiculoDetectado()
        {
            LecturaPuestosDeTrabajo = new List<LecturaPuestoDeTrabajoDto>();
        }
    }
}
