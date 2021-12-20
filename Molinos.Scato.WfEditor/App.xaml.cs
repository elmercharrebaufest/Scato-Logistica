using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Molinos.Scato.WfEditor.ViewModel;

namespace Molinos.Scato.WfEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {

            FrameworkElement.LanguageProperty.OverrideMetadata(

                typeof(FrameworkElement),

                new FrameworkPropertyMetadata(

                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var locator = (ViewModelLocator) Resources["ViewModelLocator"];
            var log = locator.GetLogger(GetType());
            string idError = DateTime.Now.ToString("yyyyMMddHHmmss");
            log.Error(e.Exception, "Ha ocurrido una excepción no manejada: {0}", idError);
            MessageBox.Show(string.Format(WfEditor.Properties.Resources.ErrorGenerico, idError), WfEditor.Properties.Resources.ItemvalidacionError, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Shutdown();
        }

    }
}
