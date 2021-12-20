using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Activities;
using System.Xaml;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioWorkflows : IServicioWorkflows
    {
        private readonly IRepositorio repositorio;
        private readonly IConversor conversor;
        private readonly ILogger log;

        public ServicioWorkflows(IRepositorio repositorio, IConversor conversor, ILogger log)
        {
            this.repositorio = repositorio;
            this.conversor = conversor;
            this.log = log;
        }

        public IList<WorkflowDto> ListarWorkflows()
        {
            log.Debug("Listando workflows...");
            return conversor.ConvertirList<Workflow, WorkflowDto>(repositorio.Listar<Workflow>());
        }

        public IList<WorkflowDto> ListarWorkflowsPorCentro(int centroId)
        {
            log.Debug("Listando workflows...");
            return conversor.ConvertirList<Workflow, WorkflowDto>(repositorio.Listar<Workflow>(x => x.Centro.Id == centroId));
        }

        public IList<WorkflowDefinicionDto> ListarDefinicionesWorkflow(int idWorkflow)
        {
            log.Debug("Listando definiciones del workflow {0}", idWorkflow);
            var definiciones = repositorio.Listar<WorkflowDefinicion>(x => x.Workflow.Id == idWorkflow);
            return conversor.ConvertirList<WorkflowDefinicion, WorkflowDefinicionDto>(definiciones);
        }

        public WorkflowDefinicionDto ObtenerUltimaDefinicionWorkflow(int idWorkflow)
        {
            log.Debug("Obteniendo última definicion del workflow {0}", idWorkflow);
            var definicion = repositorio.ObtenerMayor<WorkflowDefinicion, int>(
                x => x.Workflow.Id == idWorkflow,
                x => x.Id);

            return conversor.Convertir<WorkflowDefinicion, WorkflowDefinicionDto>(definicion);
        }

        public List<string> ListarActividadesPorDefinicionWorkflow(int idWorkflow)
        {
            var xamlx = repositorio.ObtenerMayor<WorkflowDefinicion, int, byte[]>(
                x => x.Workflow.Id == idWorkflow,
                x => x.Id,
                x => x.Definicion);

            var ms = new MemoryStream(xamlx) { Position = 0 };
            var service = XamlServices.Load(ms) as WorkflowService;
            var activity = service.Body;
            var children = WorkflowInspectionServices.GetActivities(activity).Where(w => w.GetType().Namespace == "Molinos.Scato.Actividades");
            return children.Select(s => s.DisplayName).Distinct().ToList();
        }

        public byte[] ObtenerArchivoDefinicionWorkflow(int idDefinicion)
        {
           log.Debug("Obteniendo archivo para definición: {0}", idDefinicion);
           return repositorio.Obtener<WorkflowDefinicion>(idDefinicion).Definicion;
        }

        public void ActualizarDefinicion(int idDefinicion, byte[] xamlx, string actividadInicial, bool activar)
        {
            var definicion = repositorio.Obtener<WorkflowDefinicion>(idDefinicion);
            if (definicion.Activa)
            {
                throw new InvalidOperationException(Textos.Error_ActualizarDefinicionWorkflowActiva);
            }
            definicion.Definicion = xamlx;
            definicion.ActividadInicial = actividadInicial;
            definicion.Activa = activar;
            repositorio.GuardarCambios();
        }

        public WorkflowDefinicionDto CrearWorkflow(WorkflowDefinicionDto definicion, byte[] xamlx)
        {
            if (repositorio.Existe<Workflow>(x => x.Codigo == definicion.Workflow.Codigo))
            {
                throw new InvalidOperationException(string.Format(Textos.Error_CrearWorkflowCodigoExistente, definicion.Workflow.Codigo));
            }
            var definicionWf = conversor.Convertir<WorkflowDefinicionDto, WorkflowDefinicion>(definicion);
            definicionWf.Workflow.Centro = repositorio.Obtener<Centro>(definicion.Workflow.CentroId);
            definicionWf.Definicion = xamlx;
            definicionWf.FechaCreacion = DateTime.Now;
            repositorio.Agregar(definicionWf);
            repositorio.GuardarCambios();
            return conversor.Convertir<WorkflowDefinicion, WorkflowDefinicionDto>(definicionWf);
        }

        public WorkflowDefinicionDto CrearDefinicionWorkflow(int idWorkflow, WorkflowDefinicionDto definicion, byte[] xamlx)
        {
            var definicionWf = conversor.Convertir<WorkflowDefinicionDto, WorkflowDefinicion>(definicion);
            definicionWf.Workflow = repositorio.Obtener<Workflow>(idWorkflow);
            definicionWf.Definicion = xamlx;
            definicionWf.FechaCreacion = DateTime.Now;
            repositorio.Agregar(definicionWf);
            repositorio.GuardarCambios();
            return conversor.Convertir<WorkflowDefinicion, WorkflowDefinicionDto>(definicionWf);
        }

        public void ActivarWorkflow(int idWorkflow, bool activar)
        {
            var workflow = repositorio.Obtener<Workflow>(idWorkflow);
            workflow.Activo = activar;
            repositorio.GuardarCambios();
        }

        public IList<CentroDto> ListarCentros(string nombreUsuario)
        {
            log.Debug("Listando centros");
            var definiciones = repositorio.Listar<Centro>(f => f.UsuariosAsociados.Any(x => x.NombreUsuario == nombreUsuario));
            return conversor.ConvertirList<Centro, CentroDto>(definiciones);
        }

        public int ObtenerCentroIdPorWorkflowId(int idWorkflow)
        {
            return repositorio.Obtener<Workflow>(idWorkflow).Centro.Id;
        }

        public bool TienePermisoEditordeWorkflow(string nombreUsuario)
        {
            return repositorio.ObtenerConsultaEscalar(new PermisoPorUsuarioyCodigo(nombreUsuario, PermisosScato.EditordeWorkflow));
        }
    }
}
