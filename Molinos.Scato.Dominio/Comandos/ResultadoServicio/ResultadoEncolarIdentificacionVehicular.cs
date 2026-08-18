using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoEncolarIdentificacionVehicular : Resultado
    {
        [DataMember]
        public ColaIdentificacionVehicularDto PrimerElementoCola { get; set; }
    }
}
