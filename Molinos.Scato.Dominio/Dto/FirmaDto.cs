using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Drawing;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FirmaDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "IngBrutosConvMultilateral")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string IngBrutosConvMultilateral { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_Direccion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Direccion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Ciudad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Ciudad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_De_Inicio")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string FechaDeInicio { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Centro_Cuit")]
        public string Cuit { get; set; }
       
        [Display(ResourceType = typeof(Textos), Name = "Almacen_CodigoSAP")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro_RazonSocial")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string RazonSocial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Logo")]
        public Byte[] Logo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Icono")]
        public Byte[] Favicon { get; set; }

        [FileSize(50000)]
        [FileTypes("jpg,jpeg,png")]
        public HttpPostedFileBase LogoFile { get; set; }

        [FileSize(50000)]
        [FileTypes("ico,jpg,jpeg,png")]
        public HttpPostedFileBase FaviconFile { get; set; }


        public  IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LogoFile != null)
            {
                var imagen = Image.FromStream(LogoFile.InputStream, true, true);
                if (imagen.Height > 50 || imagen.Width > 300)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Imagen_Grande), new[] { "LogoFile" });
                }
                LogoFile.InputStream.Position = 0;
            }

            if (FaviconFile != null)
            {
                var imagen = Image.FromStream(FaviconFile.InputStream, true, true);
                if (imagen.Height > 540 || imagen.Width > 540)
                {
                    yield return new ValidationResult(string.Format(Textos.Error_Imagen_Grande), new[] { "FaviconFile" });
                }
                FaviconFile.InputStream.Position = 0;
            }

            if (Cuit == null)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido,Textos.Centro_Cuit), new[] { "Cuit" });
            }
            else
            {
                var cuitInt = Cuit.Replace("-", string.Empty);
                if (cuitInt.Length != 11)
                {
                    yield return new ValidationResult(string.Format(Textos.CuitInvalido), new[] { "Cuit" });
                }
                else
                {
                    int calculado = CalcularDigitoCuit(cuitInt);
                    int digito;
                    if (!int.TryParse(cuitInt.Substring(10), out digito))
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { "Cuit" });
                    }

                    if (calculado != digito)
                    {
                        yield return new ValidationResult(Textos.CuitInvalido, new[] { "Cuit" });
                    }
                }
            }
        }

        private int CalcularDigitoCuit(string cuit)
        {
            var mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var nums = cuit.ToCharArray();
            var total = mult.Select((t, i) => int.Parse(nums[i].ToString(CultureInfo.InvariantCulture))*t).Sum();
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        public string BalanzaNombre { get; set; }

        public string Longitud { get; set; }

        public string Latitud { get; set; }
    }

    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return (value as HttpPostedFileBase).ContentLength <= _maxSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Textos.Error_Excede_Espacio, _maxSize);
        }
    }
    public class FileTypesAttribute : ValidationAttribute
    {
        private readonly List<string> _types;

        public FileTypesAttribute(string types)
        {
            _types = types.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Textos.Error_Formato_Archivo, String.Join(", ", _types));
        }
    }
}
