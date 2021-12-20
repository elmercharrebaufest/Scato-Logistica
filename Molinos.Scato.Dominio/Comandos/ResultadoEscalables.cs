using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoEscalables : Resultado
    {
        [DataMember]
        public TipoVehiculo? Categoria { get; set; }
    }
}
