using System;
using System.Globalization;
using System.Windows.Data;

namespace Molinos.Scato.WfEditorWeb.Conversores
{
    public class InvierteBooleanoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return !(boolValue.HasValue && boolValue.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
