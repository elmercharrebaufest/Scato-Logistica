using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.UI;

namespace Molinos.Scato.Dominio.Dto
{
    public class FiltroPanelPagoMunicipalDto : IValidatableObject
    {

        [Display(Name = "Ingreso Desde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime IngresoDesde { get; set; }

        [Display(Name = "Ingreso Hasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime IngresoHasta { get; set; }


        [Display(Name = "Workflow")]
        public string Workflow { get; set; }

        [Display(Name = "Nro Documento")]
        public string NroDocumento { get; set; }

        [Display(Name = "Patente")]
        public string Patente { get; set; }

        [Display(Name = "Pago Consumido")]
        public bool?PagoConsumido { get; set; }

        public TipoDeServicio TipoDeServicio { get; set; }

        [Display(Name = "Workflow")]
        public string WorkflowCodigo { get; set; }

        public string WorkflowDescripcion { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IngresoDesde != DateTime.MinValue && IngresoHasta < IngresoDesde)
            {
                yield return new ValidationResult(string.Format(Textos.MasGrandeQue, Textos.FechaFin, Textos.FechaInicio), new[] { "IngresoHasta" });
            }
        }
    }
}