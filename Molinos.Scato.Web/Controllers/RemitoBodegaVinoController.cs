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
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadRemitoBodegaVino)]
    public class RemitoBodegaVinoController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IRemitoBodegaVinoService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly ZSDWS_SCATO servicioSAP;

        public RemitoBodegaVinoController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IRemitoBodegaVinoService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, ZSDWS_SCATO servicioSAP)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.servicioSAP = servicioSAP;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index(string workflow, DatosUsuario datosUsuario)
        {
            log.Debug("Iniciando RemitoBodegaVinoController");
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            log.Debug("RemitoBodegaVinoController Obtener Workflow - PorCodigo");
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            log.Debug("Iniciando SetearVista");
            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(new RemitoBodegaVinoDto ( /*ProveedorId = proveedor.Id, Proveedor = proveedor.RazonSocial, ProveedorCodigoSap = proveedor.CodigoSap*/ ));
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, RemitoBodegaVinoDto orden, DatosUsuario datosUsuario)
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
                    TempData["Alerta"] = Textos.PatenteEnOtroWorkflow;
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }

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

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActRemitoBodegaVino,
                    ActividadXaml = "RemitoBodegaVino",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario

                };

                var materialId = orden.MaterialIdYPosicion.Split('|')[0];
                orden.MaterialId = !string.IsNullOrEmpty(materialId) ? Convert.ToInt32(materialId) : 0;
                orden.Posicion = orden.MaterialIdYPosicion.Split('|').Length > 1 ? orden.MaterialIdYPosicion.Split('|')[1] : "";

                int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                
                var resultadoActividad = servicioWf.RemitoBodegaVino(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
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
            SetearVista(workflow,centroId,servicio,this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
           var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.TiposVehiculoBodega = servicio.ListarTiposVehiculoBodega().ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.Materiales = new List<SelectListItem>();
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso; 
        }

        public ActionResult FiltrarMaterialesPorOrden(string workflow, int centroId, string materialesCodigoSap)
        {
            if (materialesCodigoSap == null)
            {
                log.Error("materialesCodigoSap es null para el workflow {0}", workflow);
                return Json(new SelectListItem(), JsonRequestBehavior.AllowGet);
            }
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var materialesCodigo = materialesCodigoSap.Split(',').ToList();
            var materialesSap = materialesCodigo.Select(x => x.Split('|')[0]).ToList();

            var materiales = servicio.ListarMaterialesPorWorkflowYCodigoSAP(workflowObj.Id, centroId, materialesSap);
                
            foreach (var m in materiales)
            {
                var codigo =  materialesCodigo.FirstOrDefault(x => x.Split('|')[0] == m.MaterialCodigoSap);
                m.Posicion = codigo != null ? codigo.Split('|')[1] : "";
            }

            
            return Json(materiales.ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture) + "|" + f.Posicion, f => f.MaterialDesc), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarEnSAP(string ordenDeCompra)
        {
            var mensajeError = "";
            try
            {
                var respuesta =
                    servicioSAP.ConsultaPedido(new ConsultaPedidoRequest(new ConsultaPedido {Pedido = ordenDeCompra}));

                var materialesCodSap = respuesta.ConsultaPedidoResponse.Detalles.Select(m => m.MATNR + "|" + m.EBELP).ToList();
                for (var i = 0; materialesCodSap.Count > i; i++)
                {
                    materialesCodSap[i] = materialesCodSap[i].TrimStart(new char[] { '0' });
                }

                string codigoProveedor = respuesta.ConsultaPedidoResponse.Proveedor;
                ProveedorDto proveedor = null;

                if (string.IsNullOrEmpty(codigoProveedor))
                {
                    mensajeError = String.Format(Textos.Error_PedidoNoEncontrado, ordenDeCompra);

                }
                else
                {

                    proveedor = servicio.ObtenerProveedorPorCodigoSap(codigoProveedor);
                }
                if (mensajeError == "")
                {
                    if (materialesCodSap.Count == 0)
                    {
                        mensajeError = String.Format(Textos.Error_PedidoSinMateriales, ordenDeCompra);
                    }
                }

                    return Json(new
                        {
                            proveedorId = proveedor != null ? proveedor.Id : 0,
                            proveedorDesc = proveedor != null ? proveedor.Descripcion : "",
                            materialesCodigoSAP = materialesCodSap,
                            mensajeError = mensajeError

                        }, JsonRequestBehavior.AllowGet);
                
            }
            catch(Exception)
            {
                return Json(new
                    {
                        mensajeError = Textos.Error_Generico
                    }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}