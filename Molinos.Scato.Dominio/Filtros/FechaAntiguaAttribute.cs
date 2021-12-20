using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Filtros
{
    public class FechaAntiguaAttribute : ValidationAttribute
    {

        public FechaAntiguaAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dias = Double.Parse(ConfigurationManager.AppSettings["DiasAntiguedadFechaCP"]);
            var fecha = (DateTime) value;
            if (fecha.AddDays(dias) <= DateTime.Now)
            {
                return new ValidationResult(string.Format(Textos.Error_FechaMayorA, String.Format(CultureInfo.CurrentCulture, "{0:d}", DateTime.Now.AddDays(-dias))));
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
