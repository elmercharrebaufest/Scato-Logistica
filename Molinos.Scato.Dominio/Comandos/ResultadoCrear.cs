using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCrear : Resultado
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
        public string PathSustentable { get; set; }
    }
}
