using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.WfEditorWeb.ViewModel
{
    public class AbrirWorkflowViewModel : ViewModelBase
    {
        private readonly IServicioWorkflows servicioWorkflows;
        public ICommand ObtenerCentrosCommand { get; private set; }
        public ICommand ObtenerWorkflowsCommand { get; private set; }
        public ICommand ObtenerVersionesCommand { get; private set; }
        public ICommand AbrirVersionCommand { get; private set; }
        public ICommand AbrirUltimaVersionCommand { get; private set; }

        private IList<WorkflowDto> workflows;
        private IList<WorkflowDefinicionDto> definiciones;
        private IList<CentroDto> centros;
        private int? workflowCentroId;

        public AbrirWorkflowViewModel(IServicioWorkflows servicioWorkflows)
        {
            this.servicioWorkflows = servicioWorkflows;
            ObtenerCentrosCommand = new RelayCommand(ObtenerCentros);
            ObtenerWorkflowsCommand = new RelayCommand<int>(ObtenerWorkflowsDisponibles);
            ObtenerVersionesCommand = new RelayCommand<WorkflowDto>(ObtenerDefiniciones, wf => wf != null && MostrarVersiones);
            AbrirVersionCommand = new RelayCommand<WorkflowDefinicionDto>(AbrirDefinicion, def => def != null);
            AbrirUltimaVersionCommand = new RelayCommand<WorkflowDto>(AbrirUltimaDefinicion, wf => wf != null);
        }

        //public WorkflowDefinicionDto DefinicionSeleccionada { get; set; }
        public bool MostrarVersiones { get; set; }

        private void AbrirDefinicion(WorkflowDefinicionDto seleccinada)
        {
            byte[] xamlx = servicioWorkflows.ObtenerArchivoDefinicionWorkflow(seleccinada.Id);
            MessengerInstance.Send(new WorkflowEditado { Definicion = seleccinada, Xamlx = xamlx });
        }

        private void AbrirUltimaDefinicion(WorkflowDto seleccionado)
        {
            var definicionDto = servicioWorkflows.ObtenerUltimaDefinicionWorkflow(seleccionado.Id);
            byte[] xamlx = servicioWorkflows.ObtenerArchivoDefinicionWorkflow(definicionDto.Id);
            MessengerInstance.Send(new WorkflowEditado { Definicion = definicionDto, Xamlx = xamlx });
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

        public IEnumerable<WorkflowDto> Workflows
        {
            get { return workflows; }
        }

        public IEnumerable<WorkflowDefinicionDto> DefinicionesWorkflow
        {
            get { return definiciones; }
        }

        public IEnumerable<CentroDto> Centros
        {
            get { return centros; }
        }

        private void ObtenerCentros()
        {
            centros = servicioWorkflows.ListarCentros(UsuarioEditor.UsuarioActual);
            var centro = centros.FirstOrDefault();
            if (centro != null)
            {
                WorkflowCentroId = centro.Id;
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
            workflows = servicioWorkflows.ListarWorkflowsPorCentro(centroId);
            RaisePropertyChanged(() => Workflows);
        }

        private void ObtenerDefiniciones(WorkflowDto seleccionado)
        {
            definiciones = servicioWorkflows.ListarDefinicionesWorkflow(seleccionado.Id)
                .OrderByDescending(x => x.FechaCreacion).ToList();
            RaisePropertyChanged(() => DefinicionesWorkflow);
        }
    }
}
