using System.Activities.Core.Presentation;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Molinos.Scato.WfEditorWeb.Mensajes;
using Molinos.Scato.WfEditorWeb.ViewModel;

using System.Windows.Controls;

namespace Molinos.Scato.WfEditorWeb.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    {
        public MainWindow()
        {
            InitializeComponent();
            RegistrarMetadata();
            Toolbox.Categories = ((MainWindowViewModel) DataContext).ToolboxItems;
            Messenger.Default.Register<PreguntaSiNoCancelar>(this, MostrarMessageBoxSiNoCancelar);
            Messenger.Default.Register<PreguntaOKCancelar>(this, MostrarMessageBoxOkCancelar);
            Messenger.Default.Register<AbrirWorkflow>(this, MostrarDialogoAbrirWorkflow);
            Messenger.Default.Register<GuardarWorkflowComo>(this, MostrarDialogoGuardarWorkflow);
            Messenger.Default.Register<ActivarDesactivarWorkflows>(this, MostrarDialogoActivarWorkflow);
            Messenger.Default.Register<Status>(this, status => TextoStatus.Text = status.Texto);
        }

        private void MostrarDialogoActivarWorkflow(ActivarDesactivarWorkflows mensaje)
        {
            var dialogo = new ActivarWorkflowWindow {};
            dialogo.ShowDialog();
        }

        private void MostrarDialogoGuardarWorkflow(GuardarWorkflowComo mensaje)
        {
            var dialogo = new GuardarWorkflowWindow { };
            Messenger.Default.Send(new WorkflowActual { IdWorkflow = mensaje.IdWorkflow });
            dialogo.ShowDialog();
        }

        private void MostrarDialogoAbrirWorkflow(AbrirWorkflow mensaje)
        {
            var dialogo = new AbrirWorkflowWindow {};
            dialogo.ShowDialog();
        }

        private void MostrarMessageBoxSiNoCancelar(PreguntaSiNoCancelar pregunta)
        {
            var resultado = MessageBox.Show(pregunta.Texto, pregunta.Titulo, MessageBoxButton.YesNoCancel);
            switch (resultado)
            {
                case MessageBoxResult.Yes: 
                    pregunta.Si();
                    break;
                case MessageBoxResult.No:
                    pregunta.No();
                    break;
                default:
                    if (pregunta.Cancelar != null)
                    {
                        pregunta.Cancelar();
                    }
                    break;
            }
        }

        private void MostrarMessageBoxOkCancelar(PreguntaOKCancelar pregunta)
        {
            var resultado = MessageBox.Show(pregunta.Texto, pregunta.Titulo, MessageBoxButton.OKCancel);
            switch (resultado)
            {
                case MessageBoxResult.OK:
                    pregunta.OK();
                    break;
                default:
                    pregunta.Cancelar();
                    break;
            }
        }

        private void RegistrarMetadata()
        {
            new DesignerMetadata().Register();
        }

    }

}
