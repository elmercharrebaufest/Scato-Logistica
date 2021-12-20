using System;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoPesaje : Resultado
    {
        [DataMember]
        public int Peso{ get; set; }
        [DataMember]
        public int BalanzaId { get; set; }
        
    }
}
