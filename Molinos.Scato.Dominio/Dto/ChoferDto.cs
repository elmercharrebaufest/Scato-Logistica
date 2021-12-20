using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ChoferDto: IValidatableObject
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Chofer_Apellido")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Apellido { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_TipoDocumentoIdentidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoDocumentoIdentidadId { get; set; }

        public string DescripcionCorta { get; set; }

        public string TipoDocumentoIdentidadCodigoSap { get; set; }
        public string TipoDocumentoIdentidadDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_NumeroDocumentoIdentidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroDeDocumento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Cuil")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(13, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Cuil { get; set; }

        public string NombreCompleto { get { return Nombre + " " + Apellido; } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Cuil == null)
            {
                yield return new ValidationResult(Textos.Error_Requerido, new[] { Textos.Chofer_Cuil });
            }
            else
            {
                var cuilInt = Cuil.Replace("-", string.Empty);
                if (cuilInt.Length != 11)
                {
                    yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { Textos.Chofer_Cuil });
                }
                else
                {
                    int calculado = CalcularDigitoCuil(cuilInt);
                    int digito;
                    int primerDigitoExtranjero;
                    int segundoDigitoExtranjero;
                    int.TryParse(cuilInt.Substring(0, 1), out primerDigitoExtranjero);
                    int.TryParse(cuilInt.Substring(1, 1), out segundoDigitoExtranjero);

                    if (!int.TryParse(cuilInt.Substring(10), out digito))
                    {
                        yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { Textos.Chofer_Cuil });
                    }

                    if (calculado != digito && primerDigitoExtranjero != 9 && segundoDigitoExtranjero != 9 && digito != 9)
                    {
                        yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { Textos.Chofer_Cuil });
                    }
                }
            }
        }

        private int CalcularDigitoCuil(string cuil)
        {
            var mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var nums = cuil.ToCharArray();
            var total = mult.Select((t, i) => int.Parse(nums[i].ToString(CultureInfo.InvariantCulture))*t).Sum();
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }
    }
}