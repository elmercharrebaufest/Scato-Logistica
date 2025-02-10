using FluentValidation;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Validations.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace Molinos.Scato.Dominio.Validations
{
    public class OrdenCargaInternaFasonValidator : AbstractValidator<OrdenCargaInternaFasonDto>, IValidatorEntity<OrdenCargaInternaFasonDto>

    {
        public OrdenCargaInternaFasonValidator()
        {
            ConfigurarValidacionDesdeAnotacionesDeDatos();
        }

        private void ConfigurarValidacionDesdeAnotacionesDeDatos()
        {
            var properties = typeof(OrdenCargaInternaFasonDto).GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case RequiredAttribute _:
                            RuleFor(x => property.GetValue(x))
                                .NotEmpty()
                                .WithName(GetDisplayName(property))
                                .WithMessage(GetErrorMessage((ValidationAttribute)attribute, GetDisplayName(property)));
                            break;

                        case StringLengthAttribute lengthAttr:
                            RuleFor(x => property.GetValue(x).ToString())
                                .MaximumLength(lengthAttr.MaximumLength)
                                .When(x => !string.IsNullOrEmpty(property.GetValue(x) as string))
                                .WithName(GetDisplayName(property))
                                .WithMessage(GetErrorMessage(lengthAttr, GetDisplayName(property)));
                            break;

                        case RegularExpressionAttribute regexAttr:
                            RuleFor(x => property.GetValue(x).ToString())
                                .Matches(regexAttr.Pattern)
                                .When(x => !string.IsNullOrEmpty(property.GetValue(x) as string))
                                .WithName(GetDisplayName(property))
                                .WithMessage(GetErrorMessage(regexAttr, GetDisplayName(property)));
                            break;

                    }
                }
            }
        }

        private string GetDisplayName(PropertyInfo property)
        {
            var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && displayAttribute.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name))
            {
                var resourceManager = new ResourceManager(displayAttribute.ResourceType);
                return resourceManager.GetString(displayAttribute.Name);
            }
            return property.Name;
        }

        private string GetErrorMessage(ValidationAttribute validationAttribute, string displayName = null)
        {
            if (validationAttribute.ErrorMessageResourceType != null && !string.IsNullOrEmpty(validationAttribute.ErrorMessageResourceName))
            {
                var resourceManager = new ResourceManager(validationAttribute.ErrorMessageResourceType);
                var response = string.Format(resourceManager.GetString(validationAttribute.ErrorMessageResourceName), displayName);
                return response;
            }
            return string.Empty;
        }

        private bool EsFechaValida(DateTime date)
        {
            return date >= new DateTime(1900, 1, 1) && date <= DateTime.Now;
        }
    }
}
