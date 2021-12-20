using System.Collections.Generic;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoActualizarEstadoConexion : Resultado
    {
        [DataMember]
        public List<EstadoConexionDto> EstadoConexionDto { get; set; }
    }
}
