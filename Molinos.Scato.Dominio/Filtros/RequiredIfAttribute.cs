using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Filtros
{
    /// <summary>
    /// Atributo de validación que hace que un campo sea requerido condicionalmente
    /// basado en el valor de otra propiedad del modelo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var containerType = validationContext.ObjectInstance.GetType();
            var dependentProperty = containerType.GetProperty(_dependentProperty);
            if (dependentProperty == null)
                return new ValidationResult($"Propiedad desconocida: {_dependentProperty}");

            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance);
            bool shouldBeRequired = false;
            if (_targetValue == null && dependentValue == null)
            {
                shouldBeRequired = true;
            }
            else if (_targetValue != null && _targetValue.Equals(dependentValue))
            {
                shouldBeRequired = true;
            }

            if (shouldBeRequired)
            {
                if (value == null || (value is string && string.IsNullOrWhiteSpace(value.ToString())))
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} es requerido.");
            }

            return ValidationResult.Success;
        }
    }
}
