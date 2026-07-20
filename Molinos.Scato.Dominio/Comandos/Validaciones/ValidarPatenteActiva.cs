using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos.Validaciones
{
    [DataContract]
    public class ValidarPatenteActiva : Comando
    {
        [DataMember]
        public List<string> Patentes { get; set; } = new List<string>();
    }
}