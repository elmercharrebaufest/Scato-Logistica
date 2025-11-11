using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    [DataContract]
    public class ResultadoVisecDistribuirStock : Resultado
    {
        [DataMember]
        public Dictionary<string, int> Data { get; set; }
    }
}