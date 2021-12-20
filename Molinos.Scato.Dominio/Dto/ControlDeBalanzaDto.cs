using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class ControlDeBalanzaDto
    {
        public int ControlDeBalanzaId { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        public int RecorridoId { get; set; }
        public string Patente { get; set; }
        public string TipoComercial { get; set; }
        public string Material { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Observaciones")]
        public string Observaciones { get; set; }
        public TipoPesada TipoPesada { get; set; }
        public IList<ControlDeBalanzaPesadaDto> ControlesDeBalanzasPesadas { get; set; }
        public string Error { get; set; }
    }
}