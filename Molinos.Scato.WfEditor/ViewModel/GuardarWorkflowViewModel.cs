using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.WfEditor.Mensajes;
using Molinos.Scato.WfEditor.Properties;

namespace Molinos.Scato.WfEditor.ViewModel
{
    public class GuardarWorkflowViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IServicioWorkflows servicioWorkflows;
        private bool esNuevoWorkflow;

        public ICommand ObtenerCentrosCommand { get; private set; }
        public ICommand ObtenerWorkflowsCommand { get; private set; }
        public ICommand GuardarCommand { get; set; }

        private IList<WorkflowDto> workflows;
        private IList<CentroDto> centros;
        private readonly IList<TipoDeWorkflow> tiposWorkflow;
        private int? workflowId;
        private int? workflowCentroId;
        private string workflowCodigo;
        private string workflowDescripcion;
        private TipoDeWorkflow workflowTipo;
        private bool workflowActivo;
        private DateTime definicionFechaActivacion;
        private string definicionComentarios;
        private bool definicionActiva;

        public GuardarWorkflowViewModel(IServicioWorkflows servicioWorkflows)
        {
            this.servicioWorkflows = servicioWorkflows;
            esNuevoWorkflow = false;
            ObtenerCentrosCommand = new RelayCommand(ObtenerCentros);
            ObtenerWorkflowsCommand = new RelayCommand<int>(ObtenerWorkflowsDisponibles);
            GuardarCommand = new RelayCommand(GuardarWorkflow, PuedeGuardarWorkflow);
            tiposWorkflow = new[] { TipoDeWorkflow.Ingreso, TipoDeWorkflow.Egreso };
            MessengerInstance.Register<WorkflowActual>(this, SeleccionarWorkflow);
            definicionFechaActivacion = DateTime.Now;
        }

        private void SeleccionarWorkflow(WorkflowActual mensaje)
        {
            if (mensaje.IdWorkflow != null)
            {
                WorkflowCentroId = servicioWorkflows.ObtenerCentroIdPorWorkflowId(mensaje.IdWorkflow.Value);
                WorkflowId = mensaje.IdWorkflow;
            }
        }

        public bool EsWorkflowExistente
        {
            get { return !esNuevoWorkflow; }
            set
            {
                esNuevoWorkflow = !value;
                RaisePropertyChanged(() => EsWorkflowExistente);
                RaisePropertyChanged(() => EsNuevoWorkflow);
            }
        }

        public bool EsNuevoWorkflow
        {
            get { return esNuevoWorkflow; }
        }

        public int? WorkflowId
        {
            get { return workflowId; }
            set 
            { 
                workflowId = value;
                RaisePropertyChanged(() => WorkflowId);
            }
        }

        public string WorkflowCodigo
        {
            get { return workflowCodigo; }
            set 
            { 
                workflowCodigo = value;
                RaisePropertyChanged(() => WorkflowCodigo);
            }
        }

        public string WorkflowDescripcion
        {
            get { return workflowDescripcion; }
            set
            {
                workflowDescripcion = value;
                RaisePropertyChanged(() => WorkflowDescripcion);
            }
        }

        public TipoDeWorkflow WorkflowTipo
        {
            get { return workflowTipo; }
            set
            {
                workflowTipo = value;
                RaisePropertyChanged(() => WorkflowTipo);
            }
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

        public bool WorkflowActivo
        {
            get { return workflowActivo; }
            set
            {
                workflowActivo = value;
                RaisePropertyChanged(() => WorkflowActivo);
            }
        }

        public DateTime DefinicionFechaActivacion
        {
            get { return definicionFechaActivacion; }
            set
            {
                definicionFechaActivacion = value.Date.AddHours(value.Hour).AddMinutes(value.Minute);
                RaisePropertyChanged(() => DefinicionFechaActivacion);
            }
        }

        public string DefinicionComentarios
        {
            get { return definicionComentarios; }
            set
            {
                definicionComentarios = value;
                RaisePropertyChanged(() => DefinicionComentarios);
            }
        }

        public bool DefinicionActiva
        {
            get { return definicionActiva; }
            set
            {
                definicionActiva = value;
                RaisePropertyChanged(() => DefinicionActiva);
            }
        }

        private bool PuedeGuardarWorkflow()
        {
            return (EsWorkflowExistente && WorkflowId.HasValue && !string.IsNullOrEmpty(DefinicionComentarios))
                   || (!EsWorkflowExistente 
                       && !string.IsNullOrEmpty(WorkflowCodigo)
                       && !string.IsNullOrEmpty(WorkflowDescripcion)
                       && !string.IsNullOrEmpty(DefinicionComentarios));
        }

        private void GuardarWorkflow()
        {
            var definicion = new WorkflowDefinicionDto
            {
                Comentario = DefinicionComentarios,
                Activa = DefinicionActiva,
                FechaCreacion = DateTime.Now,
                FechaActivacion = definicionFechaActivacion,
                NombreUsuario = UsuarioEditor.UsuarioActual,
            };
            if (EsNuevoWorkflow)
            {
                var workflow = new WorkflowDto
                    {
                        Codigo = WorkflowCodigo,
                        Descripcion = WorkflowDescripcion,
                        TipoDeWorkflow = WorkflowTipo,
                        Activo = WorkflowActivo,
                        CentroId = WorkflowCentroId.Value
                    };
                definicion.Workflow = workflow;
                MessengerInstance.Send(new GuardarNuevoWorkflow {Definicion = definicion});
            }
            else
            {
                MessengerInstance.Send(new GuardarWorkflowExistente {WorkflowId = WorkflowId.Value, Definicion = definicion});
            }
        }

        public IEnumerable<WorkflowDto> Workflows
        {
            get { return workflows; }
        }

        public IEnumerable<TipoDeWorkflow> TiposWorkflow
        {
            get { return tiposWorkflow; }
        }

        public IEnumerable<CentroDto> Centros
        {
            get { return centros; }
        }

        private void ObtenerCentros()
        {
            centros = servicioWorkflows.ListarCentros(UsuarioEditor.UsuarioActual);

            var centro = centros.FirstOrDefault();              
            if (centro != null && !WorkflowCentroId.HasValue)
            {
                WorkflowCentroId = centro.Id;
            }
            workflows = servicioWorkflows.ListarWorkflowsPorCentro(WorkflowCentroId.Value);
            if (workflows.FirstOrDefault() != null && !WorkflowId.HasValue)
            {
                WorkflowId = workflows.FirstOrDefault().Id;
            }

            RaisePropertyChanged(() => Workflows);
            RaisePropertyChanged(() => Centros);
        }

        private void ObtenerWorkflowsDisponibles(int centroId)
        {
            workflows = servicioWorkflows.ListarWorkflowsPorCentro(centroId);
            RaisePropertyChanged(() => Workflows);
            WorkflowId = workflows.FirstOrDefault() != null ? workflows.FirstOrDefault().Id : new int?();
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "WorkflowCodigo":
                        if (String.IsNullOrEmpty(WorkflowCodigo))
                        {
                            return string.Format(Resources.Error_Requerido, Resources.GuardarComoWorkflowCodigo);
                        }
                        if (workflows.Any(wf => wf.Codigo == WorkflowCodigo))
                        {
                            return string.Format(Resources.Error_WorkflowCodigoExistente, WorkflowCodigo);
                        }
                        break;
                    case "WorkflowDescripcion":
                        if (String.IsNullOrEmpty(WorkflowDescripcion))
                        {
                            return string.Format(Resources.Error_Requerido, Resources.GuardarComoWorkflowDescripcion);
                        }
                        break;
                    case "DefinicionComentarios":
                        if (String.IsNullOrEmpty(DefinicionComentarios))
                        {
                            return string.Format(Resources.Error_Requerido, Resources.GuardarComoDefinicionComentarios);
                        }
                        break;
                    case "WorkflowCentroId":
                        if (!WorkflowCentroId.HasValue)
                        {
                            return string.Format(Resources.Error_Requerido, Resources.GuardarComoWorkflowCentro);
                        }
                        break;
                }

                return null;
            }
        }

        public string Error 
        {
            get { return null; }
        }
    }
}
