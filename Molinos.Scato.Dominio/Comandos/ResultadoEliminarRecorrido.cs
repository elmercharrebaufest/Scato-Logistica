using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoEliminarRecorrido : Resultado
    {
        private IDictionary<string, string> advertencias = new Dictionary<string, string>();

        [DataMember]
        public IDictionary<string, string> Advertencias
        {
            get { return advertencias; }
            private set { advertencias = value; }
        }
    }
}