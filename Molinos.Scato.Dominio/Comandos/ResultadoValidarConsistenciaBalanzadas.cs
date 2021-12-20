using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidarConsistenciaBalanzadas : Resultado
    {
        [DataMember]
        public List<int> BalanzadasPerdidas { get; set; }
    }
}
