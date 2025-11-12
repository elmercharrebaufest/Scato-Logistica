using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroExcepcionPagoTasaMunicipal
    {
        [Required(ErrorMessage = "La patente es obligatoria.")]
        [RegularExpression(@"(^[A-Z]{3}[0-9]{3}$)|(^[A-Z]{2}[0-9]{3}[A-Z]{2}$)",
        ErrorMessage = "La patente debe tener el formato AAA123 o AA123AA")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(Name = "Patente")]
        public string Patente { get; set; }
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [Display(Name = "Numero document de ingreso")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(12, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroDocumentoIngreso { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un workflow.")]
        [Display(Name = "Workflow")]
        public string WorkflowCodigo { get; set; }

        public string WorkflowDescripcion { get; set; }
        public string Motivo { get; set; }

        public FiltroEditarExcepcionPagoTasaMunicipal FiltroEditar { get; set; }
    }
}