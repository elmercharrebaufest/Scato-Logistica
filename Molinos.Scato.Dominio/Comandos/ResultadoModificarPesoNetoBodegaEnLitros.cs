using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoModificarRecorridoPesoNetoBodegaEnLitros : Resultado
    {
        [DataMember]
        public int PesoNetoBodegaEnLitros { get; set; }
    }
}
