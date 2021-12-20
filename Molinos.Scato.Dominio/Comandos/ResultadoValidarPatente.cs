using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidarPatente : Resultado
    {
        [DataMember]
        public bool ReconocimientoExitoso { get; set; }
    }
}
