using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Presentation.Validation;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Activities;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Molinos.Scato.Actividades;
using Molinos.Scato.Servicios;
using Molinos.Scato.WfEditor.Ejecucion;
using Molinos.Scato.WfEditor.Mensajes;
using Molinos.Scato.WfEditor.Properties;
using Molinos.Scato.WfEditor.Views;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WfEditor.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Titulo { get; private set; }
        public ICommand NuevoWorkflowCommand { get; private set; }
        public ICommand AbrirWorkflowCommand { get; private set; }
        public ICommand CerrarWorkflowCommand { get; private set; }
        public ICommand GuardarWorkflowCommand { get; private set; }
        public ICommand GuardarWorkflowComoCommand { get; private set; }
        public ICommand GuardarActivarWorkflowCommand { get; private set; }
        public ICommand ActivarDesactivarWorkflowCommand { get; set; }
        
        private WorkflowDesigner workflowDesigner;
        private readonly ValidationErrorService servicioValidaciones;
        private WorkflowEditado workflowEditado;
        private bool workflowModificado;

        private readonly IServicioWorkflows servicioWorkflows;
        private readonly ILogger log;

        public MainWindowViewModel(IServicioWorkflows servicioWorkflows, ILogger log, string ambiente, string dominio)
        {
            Titulo = Resources.TituloEditorWorkflows + ((ambiente.Length > 0) ? " (" + ambiente + ")" : "");
            this.servicioWorkflows = servicioWorkflows;
            this.log = log;
            workflowEditado = null;
            servicioValidaciones = new ValidationErrorService();
            NuevoWorkflowDesigner();

            NuevoWorkflowCommand = new RelayCommand(NuevoWorkflow);
            AbrirWorkflowCommand = new RelayCommand(AbrirWorkflow);
            CerrarWorkflowCommand = new RelayCommand(CerrarWorkflow, () => workflowEditado != null);
            GuardarWorkflowCommand = new RelayCommand(() => GuardarWorkflow(false), PuedeGuardar);
            GuardarWorkflowComoCommand = new RelayCommand(GuardarWorkflowComo, PuedeGuardarComo);
            GuardarActivarWorkflowCommand = new RelayCommand(() => GuardarWorkflow(true), PuedeGuardar);
            ActivarDesactivarWorkflowCommand = new RelayCommand(() => ActivarDesactivarWorkflow());

            MessengerInstance.Register<WorkflowEditado>(this, CargarWorkflow);
            MessengerInstance.Register<GuardarNuevoWorkflow>(this, GuardarNuevoWorkflow);
            MessengerInstance.Register<GuardarWorkflowExistente>(this, GuardarWorkflowExistente);

            log.Info("Iniciando editor de Workflows...");

            new LoginWindow(Titulo, dominio).ShowDialog();
            log.Info("Validando Usuario editor de Workflows...");
            try
            {
                //if (UsuarioEditor.UsuarioActual == null || !servicioWorkflows.TienePermisoEditordeWorkflow(UsuarioEditor.UsuarioActual))
                //{
                //    log.Info("El usuario {0} no tiene permisos para ejecutar el editor de workflows", UsuarioEditor.UsuarioActual);
                //    MessageBox.Show(Resources.ErrorPermiso, Resources.ItemvalidacionError, MessageBoxButton.OK, MessageBoxImage.Warning);
                //    Application.Current.Shutdown();
                //}
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudieron verificar los permisos del usuario");
                UsuarioEditor.UsuarioActual = null;
                MessageBox.Show(Resources.PermisosError + e.Message, Resources.ErrorMensaje, MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
        }

        private bool PuedeGuardar()
        {
            return !workflowDesigner.IsInErrorState()
                   && workflowEditado != null
                   && workflowEditado.Definicion != null
                   && !workflowEditado.Definicion.Activa
                   && workflowDesigner.ActividadInicial() != null;
        }
        
        private bool PuedeGuardarComo()
        {
            return !workflowDesigner.IsInErrorState() 
                && workflowEditado != null
                && workflowDesigner.ActividadInicial() != null;
        }

        public UIElement DesignerView
        {
            get { return workflowDesigner.View; }
        }

        public UIElement PropertyInspectorView
        {
            get { return workflowDesigner.PropertyInspectorView; }
        }

        public ObservableCollection<ValidationErrorInfo> ErroresValidacion
        {
            get { return servicioValidaciones.ListaErrores; }
        }

        public ToolboxCategoryItems ToolboxItems
        {
            get
            {
                return ToolboxItemsBuilder.Items()
                    .AgregarCategoria(Resources.CategoriaControlDeFlujo, new[]
                        {
                            typeof(FlowDecision),
                            typeof(FlowSwitch<>)
                        })
                    .AgregarCategoria(Resources.CategoriaActividades,
                            typeof(SalidaDeCentro).Assembly.GetTypes().Where(t => t.Namespace == typeof(SalidaDeCentro).Namespace)) 
                    
                    .Build();
            }
        }

        private void NuevoWorkflowDesigner()
        {
            servicioValidaciones.ClearValidationErrors();
            workflowEditado = null;
            workflowModificado = false;
            workflowDesigner = new WorkflowDesigner();
            workflowDesigner.ModelChanged += (sender, args) =>
                {
                    workflowModificado = true;
                };
            workflowDesigner.Context.Services.Publish<IValidationErrorService>(servicioValidaciones);

            var configurationService = workflowDesigner.Context.Services.GetService<DesignerConfigurationService>();
            configurationService.TargetFrameworkName = new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 5));
            configurationService.LoadingFromUntrustedSourceEnabled = true;
        }

        private void NuevoWorkflow()
        {
            log.Debug("Inicalizando nuevo workflow...");
            NuevoWorkflowDesigner();
            workflowEditado = new WorkflowEditado();
            workflowDesigner.Load(new WorkflowService {Body = new Flowchart()});
            RaiseDesignerChanged();
        }

        private void AbrirWorkflow()
        {
            log.Debug("Abriendo workflow...");
            if (workflowEditado != null)
            {
                CerrarWorkflow();
            }

            if (workflowEditado == null)
            {
                MessengerInstance.Send(new AbrirWorkflow());
            }  
        }

        private void CargarWorkflow(WorkflowEditado workflow)
        {
            log.Debug("Cargando definición workflow {0}", workflow.Definicion.Id);
            NuevoWorkflowDesigner();
            workflowEditado = workflow;
            workflowDesigner.LoadXaml(workflowEditado.Xamlx);
            RaiseDesignerChanged();
        }

        private void CerrarWorkflow()
        {
            log.Debug("Cerrando definición workflow...");
            if (workflowModificado)
            {
                MessengerInstance.Send(new PreguntaSiNoCancelar
                    {
                        Titulo = Resources.GuardarCambiosTitulo,
                        Texto = Resources.GuardarCambiosMensaje,
                        Si = () =>
                            {
                                if (PuedeGuardar())
                                {
                                    GuardarWorkflow(false);
                                }
                                else
                                {
                                    GuardarWorkflowComo();
                                }
                                NuevoWorkflowDesigner();
                                RaiseDesignerChanged();
                            },
                        No = () =>
                            {
                                NuevoWorkflowDesigner();
                                RaiseDesignerChanged();
                            },
                    });
            }
            else
            {
                NuevoWorkflowDesigner();
                RaiseDesignerChanged();
            }
        }

        private void GuardarWorkflow(bool activar)
        {
            log.Debug("Guardando workflow...");
            var continuar = !activar;
            if (activar)
            {
                MessengerInstance.Send(new PreguntaOKCancelar
                    {
                        Titulo = Resources.GuardarCambiosTitulo,
                        Texto = Resources.ActivarMensaje,
                        OK = () => continuar = true
                    });
            }

            if(continuar)
            {
                workflowEditado.Xamlx = workflowDesigner.SaveXaml();
                servicioWorkflows.ActualizarDefinicion(workflowEditado.Definicion.Id, workflowEditado.Xamlx, workflowDesigner.ActividadInicial(), activar);
                workflowEditado.Definicion.Activa = activar;
                workflowModificado = false;
                MessengerInstance.Send(new Status { Texto = Resources.WorkflowGuardado });
            }
        }

        private void GuardarWorkflowComo()
        {
            MessengerInstance.Send(new GuardarWorkflowComo { IdWorkflow = workflowEditado.Definicion != null ? workflowEditado.Definicion.Workflow.Id : (int?) null });
        }

        private void GuardarNuevoWorkflow(GuardarNuevoWorkflow mensaje)
        {
            workflowEditado.Xamlx = workflowDesigner.SaveXaml();
            mensaje.Definicion.ActividadInicial = workflowDesigner.ActividadInicial();
            workflowEditado.Definicion = servicioWorkflows.CrearWorkflow(mensaje.Definicion, workflowEditado.Xamlx);
            workflowModificado = false;
        }

        private void GuardarWorkflowExistente(GuardarWorkflowExistente mensaje)
        {
            workflowEditado.Xamlx = workflowDesigner.SaveXaml();
            mensaje.Definicion.ActividadInicial = workflowDesigner.ActividadInicial();
            workflowEditado.Definicion = servicioWorkflows.CrearDefinicionWorkflow(mensaje.WorkflowId, mensaje.Definicion, workflowEditado.Xamlx);
            workflowModificado = false;
        }

        private void ActivarDesactivarWorkflow()
        {
            MessengerInstance.Send(new ActivarDesactivarWorkflows());
        }

        private void RaiseDesignerChanged()
        {
            RaisePropertyChanged(() => DesignerView);
            RaisePropertyChanged(() => PropertyInspectorView);
        }
    }
}
