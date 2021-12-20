using System;
using System.Globalization;
using System.Windows.Data;
using Molinos.Scato.WfEditor.Properties;

namespace Molinos.Scato.WfEditor.Conversores
{
    public class BooleanoASiNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleano = value as bool?;
            return booleano.HasValue && booleano.Value
                       ? Resources.Si
                       : Resources.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
