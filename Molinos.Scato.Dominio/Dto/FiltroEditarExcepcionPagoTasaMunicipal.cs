using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroEditarExcepcionPagoTasaMunicipal
    {
        [RegularExpression(@"(^[A-Z]{3}[0-9]{3}$)|(^[A-Z]{2}[0-9]{3}[A-Z]{2}$)",
        ErrorMessage = "La patente debe tener el formato AAA123 o AA123AA")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(Name = "Patente")]
        public string PatenteActual { get; set; }
        public string NombreUsuario { get; set; }

        [Display(Name = "Numero document de ingreso")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(12, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroDocumentoIngresoActual { get; set; }


        [Display(Name = "Workflow")]
        public string WorkflowCodigoActual { get; set; }

        public string WorkflowDescripcionActual { get; set; }

        public int Id { get; set; }
    }
}