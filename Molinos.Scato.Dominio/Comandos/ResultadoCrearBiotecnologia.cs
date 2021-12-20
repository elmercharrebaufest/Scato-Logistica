using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCrearBiotecnologia : ResultadoCrear
    {
        [DataMember]
        public CamaraFormatoDeArchivo FormatoDeArchivo { get; set; }
    }
}
