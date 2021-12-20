using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoEliminarNotificaciones : Resultado
    {
        [DataMember]
        public IList<string> Grupos { get; set; }
    }
}
