using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Molinos.Scato.WfEditorWeb.ViewModel;

namespace Molinos.Scato.WfEditorWeb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
            if (e.Exception.GetType().ToString() != "System.TypeLoadException")
            {
                var locator = (ViewModelLocator)Resources["ViewModelLocator"];
                var log = locator.GetLogger(GetType());
                string idError = DateTime.Now.ToString("yyyyMMddHHmmss");
                log.Error(e.Exception, "Ha ocurrido una excepción no manejada: {0}", idError);
                MessageBox.Show(string.Format(Molinos.Scato.WfEditorWeb.Properties.Resources.ErrorGenerico, idError), Molinos.Scato.WfEditorWeb.Properties.Resources.ItemvalidacionError, MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
            e.Handled = true;
        }

    }
}
