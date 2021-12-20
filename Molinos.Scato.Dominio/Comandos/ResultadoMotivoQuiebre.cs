using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoMotivoQuiebre : Resultado
    {
        [DataMember]
        public List<string> PuestosDeTrabajo { get; set; } = new List<string>();

        [DataMember]
        public List<string> Fotos { get; set; } = new List<string>();
    }
}
