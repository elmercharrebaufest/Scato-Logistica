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
    [Autorizacion(PermisosScato.ActividadIngresoRemito)]
    public class IngresoRemitoController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresoRemitoService> factory;
        private readonly IListaDeWorkflows workflows;

        public IngresoRemitoController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresoRemitoService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
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
            var ordenDeDescarga = servicio.ObtenerNumeroOrdenDeDescargaGenerado(datosUsuario.CentroId);
            SetearVista(workflowObj);
            return View(new RemitoDto{OrdenDeDescarga = ordenDeDescarga, FechaOD = DateTime.Today, EsRemitoProveedor = false});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, RemitoDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var remito = servicio.ObtenerRemitoPorNroRemito(orden.Remito);
            if (remito != null)
            {
                ModelState.AddModelError("Remito", Textos.IngresarRemito_Existente);
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

            orden.EsRemitoProveedor = false;
            orden.OrigenCodigoSap = servicio.ObtenerCentro(orden.OrigenId).CodigoSAP;
            orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
            orden.PatenteCamion = orden.PatenteCamion != null ? orden.PatenteCamion.ToUpper() : "";
            orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";
            orden.Remito = orden.Remito.Replace('-', 'R');
            
            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);

            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresoRemito,
                    ActividadXaml = "IngresoRemito",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

            var resultadoActividad = servicioWf.IngresoRemito(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
            if (resultadoActividad.HayErrores)
            {
                ModelState.AgregarErrores(resultadoActividad);
                SetearVista(workflowObj);
                return View(orden);
            }

            return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
        }
        private void SetearVista(WorkflowDto workflow)
        {
            SetearVista(workflow, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, IServicioRepositorio servicio, ControllerBase controller)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            var almacenes = servicio.ListarAlmacenesPorCentro(workflow.CentroId);
            controller.ViewBag.Almacenes = almacenes.GroupBy(x => x.Descripcion).Select(x => new AlmacenDto { Id = x.Select(y => y.Id).FirstOrDefault(), Descripcion = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Descripcion);

            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, workflow.CentroId);
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
            controller.ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString(CultureInfo.InvariantCulture)).ToList();
        }

        public JsonResult ObtenerDatos(string numero)
        {
            try
            {
                Int64 esNumero;
                if (Int64.TryParse(numero.Substring(5,8), out esNumero))
                {
                    numero = numero.Replace('-', 'R');
                    var recorrido = servicio.ObtenerRecorridoPorNumeroDocumentoSap(numero);
                    if (recorrido != null)
                    {
                        var ordenEntrePlantas = servicio.ObtenerOrdenEntrePlantasPorInstanceId(recorrido.InstanciaWorkflow);
                        if (ordenEntrePlantas != null)
                        {
                            var datosDb = new RemitoDto
                                {
                                    Origen = recorrido.Centro.Descripcion,
                                    OrigenId = recorrido.Centro.Id,
                                    OrigenCodigoSap = recorrido.Centro.CodigoSAP,
                                    Chofer = ordenEntrePlantas.Chofer,
                                    PatenteAcoplado = ordenEntrePlantas.PatenteAcoplado,
                                    PatenteCamion = ordenEntrePlantas.PatenteCamion,
                                    TipoComercial = ordenEntrePlantas.TipoComercial,
                                    TipoComercialId = ordenEntrePlantas.TipoComercialId,
                                    Transportista =ordenEntrePlantas.Transportista,
                                    TransportistaId = ordenEntrePlantas.TransportistaId,
                                    TransportistaCuit = ordenEntrePlantas.TransportistaCuit,
                                    Material = ordenEntrePlantas.Material,
                                    MaterialId = ordenEntrePlantas.MaterialId,

                                    KmRecorrer = ordenEntrePlantas.KmRecorrer,
                                    PesoBrutoOrigen = recorrido.PesoBruto,
                                    PesoTaraOrigen = recorrido.PesoTara,
                                    PesoNetoOrigen = recorrido.PesoBruto - recorrido.PesoTara,
                                    CodEstab = recorrido.Centro.CodigoEstablecimiento,
                                    Procedencia = recorrido.Centro.LocalidadDesc,
                                    ProcedenciaId = recorrido.Centro.LocalidadId ?? 0,

                                    Remito = recorrido.NumeroDeDocumentoSap, //XBNLR
                                    DocLegalRemito = recorrido.DocumentoInternoSap //MBNLR
                                };

                            return Json(new { datosDB = datosDb }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { datosDB = -3, error = Textos.IngresarRemito_Inexistente }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { datosDB = -1, error = Textos.IngresarRemito_ErrorVacio }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { datosDB = -1, error = Textos.IngresarRemito_Error }, JsonRequestBehavior.AllowGet);
            } 
        }
    }
}