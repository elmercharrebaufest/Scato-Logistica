using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.Web.Models.ArchivosTxt
{
    public static class TxtHelper
    {
        public static string GetTxtDataRow<T>(T csvDataObject, PropertyInfo[] propertyInfos)
        {
            IEnumerable<string> valuesSorted = propertyInfos
                .Select(x => new
                {
                    Value = x.GetValue(csvDataObject, null),
                    Attribute = (TxtColumnaAttribute)Attribute.GetCustomAttribute(x, typeof(TxtColumnaAttribute), false),
                    x.PropertyType
                })
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.Order)
                .Select(x => GetPropertyValueAsString(x.Value, x.Attribute.Longitud, x.PropertyType));
            return String.Join("", valuesSorted);
        }
        private static string GetPropertyValueAsString(object propertyValue, int longitud, Type type)
        {
            string propertyValueString;
            propertyValue = propertyValue ?? string.Empty;
            if (type == typeof (DateTime))
            {
                propertyValueString = (DateTime) propertyValue == DateTime.MinValue ? string.Empty.PadLeft(longitud, '0') : ((DateTime)propertyValue).ToString("yyMMdd");
            }
            else if (type == typeof (int) || type == typeof (decimal) || type == typeof (long))
            {
                propertyValueString = propertyValue.ToString().PadLeft(longitud, '0');
            }
            else // treat as a string
            {
                propertyValueString = propertyValue.ToString().PadRight(longitud, ' ');
            }
            return propertyValueString;
        }
    }
}