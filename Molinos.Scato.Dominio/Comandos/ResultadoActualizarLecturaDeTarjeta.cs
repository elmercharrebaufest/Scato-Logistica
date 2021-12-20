using System.Collections.Generic;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoActualizarLecturaDeTarjeta : Resultado
    {
        [DataMember]
        public List<LecturaPuestoDeTrabajoDto> LecturaPuestosDeTrabajo { get; set; }

        public ResultadoActualizarLecturaDeTarjeta()
        {
            LecturaPuestosDeTrabajo = new List<LecturaPuestoDeTrabajoDto>();
        }
    }
}
