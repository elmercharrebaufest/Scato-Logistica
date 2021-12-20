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
    [Autorizacion(PermisosScato.ActividadCargarHojaDeRuta)]
    public class CargarHojaDeRutaController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarHojaDeRutaService> factory;
        private readonly IListaDeWorkflows workflows;

        public CargarHojaDeRutaController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarHojaDeRutaService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(string workflow, DatosUsuario datosUsuario)
        {
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var codigoCentro = servicio.ObtenerCentro(datosUsuario.CentroId).CodigoSAP ?? "";
            var numero = codigoCentro + "-" + servicio.ObtenerNumeroHojaDeRutaGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            SetearVista(workflowObj);
            return View(new HojaDeRutaDto { Numero = numero });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, HojaDeRutaDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            var numeroDeOrden = servicio.ObtenerHojaDeRutaPorNumero(orden.Numero);
            if (numeroDeOrden != null)
            {
                ModelState.AddModelError("Numero", Textos.HojaDeRuta_Existente);
                SetearVista(workflowObj);
                return View(orden);
            }
            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                ModelState.AddModelError("", Textos.OrdenCargaInterna_PatenteEnOtroWorkflow);
                SetearVista(workflowObj);
                return View(orden);
            }
            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                SetearVista(workflowObj);
                return View(orden);
            }
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                SetearVista(workflowObj);
                return View(orden);
            }
            if (ModelState.IsValid)
            {
                orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
                orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActCargarHojaDeRuta,
                    ActividadXaml = "CargarHojaDeRuta",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                var resultadoActividad = servicioWf.CargarHojaDeRuta(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                if (resultadoActividad.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoActividad);
                    SetearVista(workflowObj);
                    return View(orden);
                }

                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
            }
            SetearVista(workflowObj);
            return View(orden);
        }

        private void SetearVista(WorkflowDto workflow)
        {
            SetearVista(workflow, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, IServicioRepositorio servicio, ControllerBase controller)
        {
           
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(workflow.CentroId);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => f.TipoVehiculo.DisplayEnum(), f => f.TipoVehiculo.DisplayText());

            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, workflow.CentroId);
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
        }
    }
}