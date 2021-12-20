using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Filtros
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        private string _customFormat;
        private string _customFormatWithTime;

        public DateTimeModelBinder(string customFormat, string customFormatWithTime)
        {
            _customFormat = customFormat;
            _customFormatWithTime = customFormatWithTime;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (bindingContext.ModelName.Contains("Hora"))
            {
                return DateTime.ParseExact(value.AttemptedValue, _customFormatWithTime, CultureInfo.InvariantCulture);
            }

            return DateTime.ParseExact(value.AttemptedValue, _customFormat, CultureInfo.InvariantCulture);
        }
    }
}