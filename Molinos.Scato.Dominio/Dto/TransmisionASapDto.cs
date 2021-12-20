using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{

    [DataContract]
    [KnownType("TiposDeTransmisiones")]
    public class TransmisionASapDto
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public Guid InstanciaWorkflow { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_Estado")]
        public EstadoTransmisionASap Estado { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_Fecha")]
        public DateTime Fecha { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_FuncionSap")]
        public FuncionSAP FuncionSap { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_MensajeError")]
        public string MensajeError { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_TipoDocumento")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_NumeroDocumento")]
        public string NumeroDocumento { get; set; }
        [DataMember]
        [Display(ResourceType = typeof (Textos), Name = "TransmisionASap_Patente")]
        public string Patente { get; set; }

        public static Type[] TiposDeTransmisiones()
        {
            var tipoTrasmision = typeof(TransmisionASapDto);
            return tipoTrasmision.Assembly.GetTypes().Where(tipoTrasmision.IsAssignableFrom).ToArray();
        }
    }
}
