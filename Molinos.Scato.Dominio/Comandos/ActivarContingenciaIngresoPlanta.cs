using Molinos.Scato.Dominio.Enums;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ActivarContingenciaIngresoPlanta : Comando
    {
        [DataMember]
        public int PuestoId { get; set; }
        [DataMember]
        public bool Granos { get; set; }
    }
}
