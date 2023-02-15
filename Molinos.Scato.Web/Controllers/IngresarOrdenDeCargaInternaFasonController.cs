using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarOrdenCargaInternaFason)]
    public class IngresarOrdenCargaInternaFasonController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factory;
        private readonly IListaDeWorkflows workflows;

        public IngresarOrdenCargaInternaFasonController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(string workflow, DatosUsuario datosUsuario, int cargaDeCupoId = 0)
        {
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            SetearVista(workflowObj, datosUsuario.CentroId);

            var numeroOrden = servicio.ObtenerNumeroDocumentoFasonGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            var orden = new OrdenCargaInternaFasonDto {FechaEmision = DateTime.Now, NumeroOrden = numeroOrden };
            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                orden.PatenteCamion = cupo.Patente;
                orden.MaterialId = cupo.MaterialId;
                orden.MaterialDesc = cupo.MaterialDescripcion;
            }
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {
            var workflowObje = servicio.ObtenerWorkflowPorCodigo(workflow);
            Validar(orden, datosUsuario);

            if (!ModelState.IsValid)
            {
                SetearVista(workflowObje, datosUsuario.CentroId);
                return View(orden);
            }

            if (orden.PatenteCamion != null)
            {
                orden.PatenteCamion = orden.PatenteCamion.ToUpper();
            }
            if (orden.PatenteAcoplado != null)
            {
                orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
            }

            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                TempData["Alerta"] = Textos.PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var controlRecorrido = new ControlRecorridoDto
                    {
                    Actividad = Textos.ActIngresarOrdenCargaInternaFason,
                    ActividadXaml = "IngresarOrdenCargaInternaFason",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
            
            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaInternaFason(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;

            if (resultadoActividad != null && !resultadoActividad.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones", new {id = resultadoActividad.InstanciaWorkflowId});
            }

            ModelState.AgregarErrores(resultadoActividad);
            SetearVista(workflowObje, datosUsuario.CentroId);
            return View(orden);
        }

        private void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
            controller.ViewBag.WorkflowId = workflow.Id;
            controller.ViewBag.MaterialesDerivadoGranario = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();
        }

        protected static string MascaraCuit(string entrada)
        {
            if (entrada.Length != 11)
            {
                return entrada;
            }
            return entrada.Substring(0, 2) + "-" + entrada.Substring(2, 8) + "-" + entrada.Substring(10);
        }

        protected virtual void Validar(OrdenCargaInternaFasonDto orden, DatosUsuario usuario)
        {
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;

            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
            }

            if (!material.EsDerivadoGranario && (orden.PlantaDGDestino.HasValue || orden.OrdenDomicilioDestino.HasValue || orden.PagadorFleteId.HasValue))
            {
                ModelState.AddModelError("MaterialId", "El material no es un derivado granario.");
            }

            if (material.EsDerivadoGranario && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (material.EsDerivadoGranario && !orden.OrdenDomicilioDestino.HasValue)
            {
                ModelState.AddModelError("OrdenDomicilioDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_OrdenDomicilioDestino));
            }

            if (material.EsDerivadoGranario && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0))
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }
        }
    }
}