using System;
using System.Globalization;
using System.Windows.Controls;
using Molinos.Scato.WfEditor.Properties;

namespace Molinos.Scato.WfEditor.Validaciones
{
    public class Requerido : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valido = !string.IsNullOrEmpty(value as String);
            return new ValidationResult(
                valido, valido ? null : Resources.Error_Requerido
                );
        }
    }
}
