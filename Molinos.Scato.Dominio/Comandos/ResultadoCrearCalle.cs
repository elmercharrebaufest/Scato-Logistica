using Molinos.Scato.Dominio.Enums;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCrearCalle : ResultadoCrear
    {
        [DataMember]
        public int Disponibilidad { get; set; }
        [DataMember]
        public bool DisponibilidadCalles { get; set; }
        [DataMember]
        public int CentroId { get; set; }
        [DataMember]
        public int CalleId { get; set; }
        [DataMember]
        public TipoCalidad Calidad { get; set; }
        public string CalleNombre { get; set; }
    }
}
