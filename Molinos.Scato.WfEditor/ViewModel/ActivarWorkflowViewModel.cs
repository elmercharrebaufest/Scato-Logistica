using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.WfEditor.Mensajes;
using Molinos.Scato.WfEditor.Properties;

namespace Molinos.Scato.WfEditor.ViewModel
{
    public class ActivarWorkflowViewModel : ViewModelBase
    {
        private readonly IServicioWorkflows servicioWorkflows;
        public ICommand ObtenerCentrosCommand { get; private set; }
        public ICommand ObtenerWorkflowsCommand { get; private set; }
        public ICommand ActivarWorkflowCommand { get; private set; }
        public ICommand DesactivarWorkflowCommand { get; private set; }

        private IList<ObservableWorkflowDto> workflows;
        private IList<CentroDto> centros;
        private int? workflowCentroId;

        public ActivarWorkflowViewModel(IServicioWorkflows servicioWorkflows)
        {
            this.servicioWorkflows = servicioWorkflows;
            ObtenerCentrosCommand = new RelayCommand(ObtenerCentros);
            ObtenerWorkflowsCommand = new RelayCommand<int>(ObtenerWorkflowsDisponibles);
            ActivarWorkflowCommand = new RelayCommand<ObservableWorkflowDto>(ActivarWorkflow, wf => PuedeActivar(wf, true));
            DesactivarWorkflowCommand = new RelayCommand<ObservableWorkflowDto>(DesactivarWorkflow, wf => PuedeActivar(wf, false));
        }

        private void ActivarWorkflow(ObservableWorkflowDto workflow)
        {
            ActivarDesactivarWorkflow(workflow, true, 
                Resources.ActivarWorkflowTitulo, 
                Resources.ActivarWorkflowMensaje);
        }

        private void DesactivarWorkflow(ObservableWorkflowDto workflow)
        {
            ActivarDesactivarWorkflow(workflow, false, 
                Resources.DesactivarWorkflowTitulo,
                Resources.DesactivarWorkflowMensaje);
        }

        private bool PuedeActivar(ObservableWorkflowDto workflow, bool activar)
        {
            return workflow != null && workflow.Activo != activar;
        }

        private void ActivarDesactivarWorkflow(ObservableWorkflowDto workflow, bool activar, string tituloMensaje, string textoMensaje)
        {
            MessengerInstance.Send(new PreguntaOKCancelar
                {
                    Titulo = tituloMensaje,
                    Texto = string.Format(textoMensaje, workflow.Descripcion),
                    OK = () =>
                        {
                            servicioWorkflows.ActivarWorkflow(workflow.Id, activar);
                            workflow.Activo = activar; 
                        }
                });
        }

        public int? WorkflowCentroId
        {
            get { return workflowCentroId; }
            set
            {
                workflowCentroId = value;
                RaisePropertyChanged(() => WorkflowCentroId);
            }
        }

        public IList<ObservableWorkflowDto> Workflows
        {
            get { return workflows; }
        }

        public IEnumerable<CentroDto> Centros
        {
            get { return centros; }
        }

        private void ObtenerCentros()
        {
            centros = servicioWorkflows.ListarCentros(UsuarioEditor.UsuarioActual);

            if (centros.FirstOrDefault() != null)
            {
                WorkflowCentroId = centros.FirstOrDefault().Id;
                ObtenerWorkflowsDisponibles(WorkflowCentroId.Value);
            }
            else
            {
                WorkflowCentroId = new int?();
            }
            RaisePropertyChanged(() => Centros);
        }

        private void ObtenerWorkflowsDisponibles(int centroId)
        {
            workflows = servicioWorkflows.ListarWorkflowsPorCentro(centroId).OrderBy(wf => wf.Codigo).Select(wf => new ObservableWorkflowDto(wf)).ToList();
            RaisePropertyChanged(() => Workflows);
        }
    }
}
