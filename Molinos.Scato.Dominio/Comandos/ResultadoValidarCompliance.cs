using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidarCompliance : Resultado
    {
        [DataMember]
        public bool EsValido { get; protected set; }

        public ResultadoValidarCompliance()
        {
            EsValido = false;
        }
        public void Valido()
        {
            EsValido = true;
        }
    }
}
