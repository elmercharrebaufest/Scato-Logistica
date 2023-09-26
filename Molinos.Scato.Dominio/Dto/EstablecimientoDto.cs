using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EstablecimientoDto : IValidatableObject
    {
        public EstablecimientoDto()
        {
            CorredoresAsociados = new List<ProveedorDto>();
        }

        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NombreDeEstablecimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Proveedor { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProveedorId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Codigo")]
        [MaxLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"\d{0,6}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodigoDeEstablecimiento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Domicilio")]
        [MaxLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Domicilio { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_CodigoPostal")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoPostal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Localidad")]
        public string Localidad { get; set; }

        public string LocalidadCodigoAfip { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Localidad")]
        public int? LocalidadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Provincia")]
        public string Provincia { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Provincia")]
        public int? ProvinciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Anulado")]
        public bool Anulado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_SojaEPA")]
        public bool EsSojaEPA { get; set; }

        public int ComercialId { get; set; }

        public string Comercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Provisorio")]
        public bool EsProvisorio { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Establecimiento_Standard2")]
        public bool EsStandard2 { get; set; }

        public string Observaciones { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProveedorId == 0)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.Proveedor), new[] { "Proveedor" });
            }
        }

        [Display(ResourceType = typeof(Textos), Name = "Corredor_Acopio")]
        public string Corredor { get; set; }
        public string CorredorId { get; set; }

        public List<ProveedorDto> CorredoresAsociados { get; set; }

        public string ListaCorredoresAsociados
        {
            get
            {
                return String.Join(",", this.CorredoresAsociados.ToList().Select(x => x.Descripcion));
            }
        }
    }
}