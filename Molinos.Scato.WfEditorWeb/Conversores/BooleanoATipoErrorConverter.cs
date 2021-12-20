using System;
using System.Globalization;
using System.Windows.Data;
using Molinos.Scato.WfEditorWeb.Properties;

namespace Molinos.Scato.WfEditorWeb.Conversores
{
    public class BooleanoATipoErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isWarning = value as bool?;
            return isWarning != null && isWarning.Value
                       ? Resources.ItemValidacionWarning
                       : Resources.ItemvalidacionError;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
