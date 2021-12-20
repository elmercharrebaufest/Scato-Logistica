using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCargarBinesSalida : Resultado
    {
        [DataMember]
        public int PesoTaraBodega { get; set; }
    }
}
