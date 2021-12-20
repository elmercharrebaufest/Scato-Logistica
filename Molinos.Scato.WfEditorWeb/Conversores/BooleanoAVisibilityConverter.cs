using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Molinos.Scato.WfEditorWeb.Conversores
{
    public class BooleanoAVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return boolValue != null && boolValue.Value
                       ? Visibility.Visible
                       : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
