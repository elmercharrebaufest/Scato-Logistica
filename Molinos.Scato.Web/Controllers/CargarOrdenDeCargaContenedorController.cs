using System.Collections.Generic;
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
    [Autorizacion(PermisosScato.ActividadCargarOrdenDeCargaContenedor)]
    public class CargarOrdenDeCargaContenedorController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarOrdenDeCargaContenedorService> factory;
        private readonly IListaDeWorkflows workflows;

        public CargarOrdenDeCargaContenedorController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarOrdenDeCargaContenedorService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
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
            var numero = servicio.ObtenerNumeroOrdenDeCargaContenedorGenerado(datosUsuario.CentroId);
            SetearVista(workflowObj);
            return View(new OrdenDeCargaContenedorDto {OrdenDeCargaContenedor = numero});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenDeCargaContenedorDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            orden.PesoContenedorEntrada = servicio.ObtenerTaraContenedor(orden.ContenedorEntradaId).PesoTara;
            orden.PesoContenedorSalida = servicio.ObtenerTaraContenedor(orden.ContenedorSalidaId).PesoTara;

            var numeroDeOrden = servicio.ObtenerOrdenDeCargaContenedorPorNumeroDeOrden(orden.OrdenDeCargaContenedor);
            if (numeroDeOrden != null)
            {
                ModelState.AddModelError("OrdenDeCargaContenedor", Textos.OrdenDeCargaContenedor_Existente);
                SetearVista(workflowObj);
                return View(orden);
            }

            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                SetearVista(workflowObj);
                return View(orden);
            }

            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                TempData["Alerta"] = Textos.OrdenCargaInterna_PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
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

                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActCargarOrdenDeCargaContenedor,
                        ActividadXaml = "CargarOrdenDeCargaContenedor",
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };
                                
                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                var resultadoActividad = servicioWf.CargarOrdenDeCargaContenedor(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
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
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => f.TipoVehiculo.DisplayEnum(), f => f.TipoVehiculo.DisplayText());

            var contenedoresPosibles = new List<SelectListItem>
                {
                    new SelectListItem {Selected = true, Value = "", Text = ""}
                };

            var contenedores = servicio.ListarTaraContenedores().ToSelectList(f => f.Id.Value.ToString(CultureInfo.CurrentCulture), f => f.Descripcion);
            contenedoresPosibles.AddRange(contenedores);

            controller.ViewBag.ContenedoresEntrada = contenedoresPosibles;
            controller.ViewBag.ContenedoresSalida = contenedoresPosibles;

            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, workflow.CentroId);
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
        }
    }
}