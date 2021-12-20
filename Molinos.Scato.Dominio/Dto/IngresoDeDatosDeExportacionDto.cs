using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class IngresoDeDatosDeExportacionDto : IValidatableObject
    {
        public int Id { get; set; }
        public Guid InstanciaWorkflow { set; get; }
        public int WorkflowDefinicionId { set; get; }

        [Display(ResourceType = typeof(Textos), Name = "Permiso_Embarque")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PermisoEmbarque { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Identificador_Contenedor")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string IdentificadorContenedor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Firma_Titulo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int FirmaId { get; set; }
        public string FirmaRazonSocial { get; set; }
        public string FirmaCuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Neto_Esperado")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? PesoNeto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Nacionalidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int NacionalidadId { get; set; }
        public string Nacionalidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_ATA")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TransportistaId { get; set; }
        public string Transportista { get; set; }

        public bool FiltrarPorTarjeta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(new Regex(@"^[0-9]{5}[A-Z]{2}[0-9]{8}[A-Z]{1}$").Match(PermisoEmbarque).Success))
            {
                yield return new ValidationResult(string.Format(Textos.Error_FormatoPermisoEmbarque));
            }
        }
    }
}