using System.Windows;

namespace Molinos.Scato.WfEditor.Views
{
    /// <summary>
    /// Interaction logic for AbrirWorkflowWindow.xaml
    /// </summary>
    public partial class AbrirWorkflowWindow : Window
    {
        public AbrirWorkflowWindow()
        {
            InitializeComponent();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
