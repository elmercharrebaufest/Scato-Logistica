using System;
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
    [Autorizacion(PermisosScato.ActividadRemitoBodegaUvaPropia)]
    public class RemitoBodegaUvaPropiaController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IRemitoBodegaUvaService> factory;
        private readonly IFirmaProvider configuracion;
        private readonly IListaDeWorkflows workflows;

        public RemitoBodegaUvaPropiaController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IRemitoBodegaUvaService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.configuracion = configuracion;
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
            var firmaCodigoSap = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var proveedor = servicio.ObtenerProveedorPorCodigoSap(firmaCodigoSap);
            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(new RemitoBodegaUvaDto{ProveedorId = proveedor.Id, Proveedor = proveedor.RazonSocial, ProveedorCodigoSap = proveedor.CodigoSap, Cosecha = DateTime.Today.Year.ToString(CultureInfo.InvariantCulture)});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, string descargasFinales, RemitoBodegaUvaDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (ModelState.IsValid)
            {
                if (datosUsuario.CentroId == 0)
                {
                    TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }

                if (workflows.ObtenerWorkflowPorPatente(orden.Patente) != null)
                {
                    TempData["Alerta"] = Textos.OrdenCargaInterna_PatenteEnOtroWorkflow;
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }

                var listaDescargaDeBines = descargasFinales.FromJson<DescargaDeBinesDto[]>();
                if (!listaDescargaDeBines.Any())
                {
                    ModelState.AddModelError("",Textos.Error_DescargaBines_Obligatorio);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }
                orden.DescargasDeBines = listaDescargaDeBines;

                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;

                orden.Patente = orden.Patente != null ? orden.Patente.ToUpper() : "";
                var materialId = orden.MaterialIdYPosicion.Split('|')[0];
                orden.MaterialId = !string.IsNullOrEmpty(materialId) ? Convert.ToInt32(materialId) : 0;
                orden.Posicion = orden.MaterialIdYPosicion.Split('|').Length > 1 ? orden.MaterialIdYPosicion.Split('|')[1] : "";

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActRemitoBodegaUvaPropia,
                    ActividadXaml = "RemitoBodegaUvaPropia",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                var resultadoActividad = servicioWf.RemitoBodegaUva(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                if (resultadoActividad.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoActividad);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });                
            }

            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(orden);
        }

        private void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var vehiculos = servicio.ListarTiposVehiculoBodega();
            var tiposVehiculoBodega = new List<SelectListItem>();
            tiposVehiculoBodega.AddRange(vehiculos.Select(vehiculo => new SelectListItem {Value = vehiculo.Id.ToString(CultureInfo.InvariantCulture), Text = vehiculo.Descripcion}));
            controller.ViewBag.TiposVehiculoBodega = tiposVehiculoBodega;

            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Vinedos = servicio.ListarVinedosPropios().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.Materiales = new List<SelectListItem>();
            controller.ViewBag.TiposDeBines = servicio.ListarMaterialesBin(workflow.Id, centroId, null).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
        }

        public ActionResult FiltrarMaterialesPorVinedo(string workflow, int centroId, int vinedoId)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            var materiales = servicio.ListarMaterialesPorWorkflowYVinedo(workflowObj.Id, centroId, vinedoId)
                .ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
            return Json(materiales, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltrarCuartelesPorVinedo(int vinedoId)
        {
            var cuarteles = servicio.FiltrarCuartelesPorVinedo(vinedoId)
                .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Codigo);
            return Json(cuarteles, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerDescargas(int id)
        {
            IList<DescargaDeBinesDto> descargasDeBines = new List<DescargaDeBinesDto>();
            if (id > 0)
            {
                descargasDeBines = servicio.ObtenerDescargasDeBinesPorRemitoBodegaUva(id);                
            }
            return Json(descargasDeBines, JsonRequestBehavior.AllowGet);                        
        }

        public ActionResult ObtenerClasePorMaterial(int materialId)
        {
            var clase = servicio.ObtenerMaterial(materialId).Clase;
            return Json(new { resultado = clase }, JsonRequestBehavior.AllowGet);
        }
    }
}