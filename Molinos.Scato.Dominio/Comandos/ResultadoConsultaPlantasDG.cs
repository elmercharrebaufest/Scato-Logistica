using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaPlantasDG : Resultado
    {
        [DataMember]
        public List<int> Plantas { get; set; }
    }

    
}
