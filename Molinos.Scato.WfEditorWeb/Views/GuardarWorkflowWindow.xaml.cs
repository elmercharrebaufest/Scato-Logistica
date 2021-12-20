using System.Windows;

namespace Molinos.Scato.WfEditorWeb.Views
{
    /// <summary>
    /// Interaction logic for GuardarWorkflowWindow.xaml
    /// </summary>
    public partial class GuardarWorkflowWindow : Window
    {
        public GuardarWorkflowWindow()
        {
            InitializeComponent();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
