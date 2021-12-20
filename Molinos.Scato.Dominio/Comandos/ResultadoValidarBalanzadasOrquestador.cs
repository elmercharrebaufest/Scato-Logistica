using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidarBalanzadasOrquestador : Resultado
    {
        [DataMember]
        public List<int> BalanzadasPerdidas { get; set; }
    }
}
