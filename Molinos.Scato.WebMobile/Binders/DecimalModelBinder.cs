using System;
using System.Globalization;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Binders
{
    /// <summary>
    /// Model Binder personalizado para manejar decimales con diferentes separadores (punto y coma)
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            if (valueResult == null || string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            {
                if (bindingContext.ModelMetadata.IsNullableValueType)
                    return null;
                
                return 0m;
            }

            var attemptedValue = valueResult.AttemptedValue.Trim();

            try
            {
                if (decimal.TryParse(attemptedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }

                if (decimal.TryParse(attemptedValue, NumberStyles.Number, new CultureInfo("es-AR"), out result))
                {
                    return result;
                }

                var normalizedValue = attemptedValue.Replace(',', '.');
                if (decimal.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                modelState.Errors.Add($"El valor '{attemptedValue}' no es un número decimal válido");
                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
                
                return bindingContext.ModelMetadata.IsNullableValueType ? (decimal?)null : 0m;
            }
            catch (Exception ex)
            {
                modelState.Errors.Add(ex);
                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
                return bindingContext.ModelMetadata.IsNullableValueType ? (decimal?)null : 0m;
            }
        }
    }
}
