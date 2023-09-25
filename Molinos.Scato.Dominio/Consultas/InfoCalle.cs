using Molinos.Scato.Dominio.Enums;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Consultas
{
    [DataContract]
    public class InfoCalle
    {
        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public TipoCalle TipoCalle { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public bool EsIncluidoAutomatizmo { get; set; }

        [DataMember]
        public string Material { get; set; }

        [DataMember]
        public string Variedad { get; set; }

        [DataMember]
        public string Hidraulica { get; set; }
        [DataMember]
        public bool EsPaseDirecto { get; set; }
       
    }
}
