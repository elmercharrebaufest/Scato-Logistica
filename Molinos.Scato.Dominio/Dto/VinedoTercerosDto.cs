using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VinedoTercerosDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoTerceros_NumeroINV")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(8,ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[\w\-]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloAlfanumerico")]
        public string NumeroINV { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoTerceros_Titular")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoTerceros_IngresosBrutos")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string IngresosBrutos { get; set; }
        public string Proveedor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProveedorId { get; set; }
        public string CUITProveedor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(13, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "VinedoTerceros_CUITTitular")]
        public string CUITTitular { get; set; }
        public bool esProveedorPR { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "SubZona")]
        public string SubZona { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "SubZona")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? SubZonaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Zona")]
        public string Zona { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Zona")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? ZonaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "Calidad")]
        public string Calidad { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CUITTitular == null)
            {
                yield return new ValidationResult(Textos.Error_Requerido, new[] { Textos.Transportista_Cuit });
            }
            else
            {
                var cuitInt = CUITTitular.Replace("-", string.Empty);
                if (cuitInt.Length != 11)
                {
                    yield return new ValidationResult(Textos.CuitInvalido, new[] { Textos.Transportista_Cuit });
                }
                else
                {
                    int calculado = CalcularDigitoCuit(cuitInt);
                    int digito;
                    if (!int.TryParse(cuitInt.Substring(10), out digito))
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { Textos.Transportista_Cuit });
                    }

                    if (calculado != digito)
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { Textos.Transportista_Cuit });
                    }
                }
            }
        }

        private int CalcularDigitoCuit(string cuit)
        {
            var mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var nums = cuit.ToCharArray();
            var total = mult.Select((t, i) => int.Parse(nums[i].ToString(CultureInfo.InvariantCulture)) * t).Sum();
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

    }
}
