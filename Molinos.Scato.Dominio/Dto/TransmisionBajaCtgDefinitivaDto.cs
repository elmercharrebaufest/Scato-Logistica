using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{

    public class TransmisionBajaCtgDefinitivaDto
    {
        public int? Id { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Fecha")]
        public DateTime Fecha { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_MensajeError")]
        public string MensajeError { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_TipoDocumento")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_NumeroDocumento")]
        public string NumeroDocumento { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Patente")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Estado")]

        public EstadoTransmisionASap EstadoCtg { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_EstadoCtgDefinitivo")]
        public EstadoTransmisionASap EstadoCtgDefinitivo { get; set; }
    }
}
