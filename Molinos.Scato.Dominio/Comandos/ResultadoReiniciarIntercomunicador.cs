using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoReiniciarIntercomunicador : Resultado
    {
        [DataMember]
        public bool ReinicioExitoso { get; set; }

        [DataMember]
        public string Servidor { get; set; }
    }
}
