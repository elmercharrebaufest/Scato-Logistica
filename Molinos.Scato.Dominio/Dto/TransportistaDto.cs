using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TransportistaDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Cuit")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_RazonSocial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string RazonSocial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Domicilio")]
        [MaxLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Domicilio { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Localidad")]
        public string Localidad { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_Localidad")]
        public int? LocalidadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista_Provincia")]
        public string Provincia { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_Provincia")]
        public int? ProvinciaId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_MedioDePago")]
        public MedioDePago MedioDePago { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cuit == null)
            {
                yield return new ValidationResult(Textos.Error_Requerido, new[] { Textos.Transportista_Cuit });
            }
            else
            {
                var cuitInt = Cuit.Replace("-", string.Empty);
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
