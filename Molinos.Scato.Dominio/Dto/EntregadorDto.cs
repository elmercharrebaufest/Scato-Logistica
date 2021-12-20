using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EntregadorDto: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador_Tratamiento")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Tratamiento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Transportista_RazonSocial")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string RazonSocial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Entregador_CodigoSAPCondicionFiscal")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSAPCondicionFiscal { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Chofer_Cuil")]
        [StringLength(13, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Cuil { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Pais")]
        public string Pais { get; set; }
        public int? PaisId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_Provincia")]
        public string Provincia { get; set; }
        public int? ProvinciaId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_Localidad")]
        public string Localidad { get; set; }
        public int? LocalidadId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Transportista_Domicilio")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Domicilio { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Entregador_Tipo")]
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string TipoEntregador { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador_ToleranciaPorc")]
        public decimal? ToleranciaEnPorcentaje { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador_ToleranciaKg")]
        public decimal? ToleranciaEnKg { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador_EnvioAutomaticoMail")]
        public bool EnvioAutomaticoMail { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador_Pesada")]
        public bool Pesada { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entrgador_Analisis")]
        public bool Analisis { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Mail")]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Mail")]
        [DataType(DataType.EmailAddress)]
        [StringLength(45, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Mail { get; set; }
        public bool Activo { get; set; }

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
                    yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { "Cuil" });
                }
                else
                {
                    int calculado = CalcularDigitoCuil(cuilInt);
                    int digito;
                    if (!int.TryParse(cuilInt.Substring(10), out digito))
                    {
                        yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { Textos.Chofer_Cuil });
                    }

                    if (calculado != digito)
                    {
                        yield return new ValidationResult(Textos.Chofer_CuilInvalido, new[] { "Cuil" });
                    }
                }
            }

            if (EnvioAutomaticoMail && !Analisis && !Pesada)
            {
                yield return new ValidationResult(Textos.Entregador_MailObligatorio, new[] { "Pesada" });
            }

            if (EnvioAutomaticoMail && string.IsNullOrEmpty(Mail))
            {
                yield return new ValidationResult(Textos.Entregador_Error_MailOpcion, new[] { "Mail" });
            }
        }

        private int CalcularDigitoCuil(string cuil)
        {
            var mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var nums = cuil.ToCharArray();
            var total = mult.Select((t, i) => int.Parse(nums[i].ToString(CultureInfo.InvariantCulture)) * t).Sum();
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }
    }
}
