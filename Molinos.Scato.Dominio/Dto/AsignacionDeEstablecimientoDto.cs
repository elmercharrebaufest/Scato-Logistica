using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class AsignacionDeEstablecimientoDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public string Proveedor { set; get; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public Guid InstanceId { set; get; }
        public int WorkflowDefinicionId { set; get; }
        public string Workflow { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Establecimiento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int RecorridoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento")]
        public int EstablecimientoId { set; get; }

        public IList<EstablecimientoDto> Establecimientos { set; get; }
        public TipoVehiculo TipoVehiculo { get; set; }

        public string NumeroDocumentoIngreso { get; set; }
        public string Patente { get; set; }
    }
}