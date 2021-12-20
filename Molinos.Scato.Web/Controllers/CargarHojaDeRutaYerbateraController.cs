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
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCargarHojaDeRutaYerbatera)]
    public class CargarHojaDeRutaYerbateraController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarHojaDeRutaYerbateraService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly IFirmaProvider configuracion;

        public CargarHojaDeRutaYerbateraController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarHojaDeRutaYerbateraService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public virtual ActionResult Index(string workflow, DatosUsuario datosUsuario)
        {
            log.Debug("Cookie Usuario: {0}", new CookieUsuario());
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                TempData["Alerta"] = Textos.Error_WorkflowSinDefinicionActiva;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            SetearVista(workflowObj, datosUsuario.CentroId);

            var firmaCodigoSap = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var hoja = servicio.ObtenerHojaDeRutaYerbateraVacia(datosUsuario.CentroId, workflow, firmaCodigoSap, firmaCodigoSap);
            return View(hoja);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public virtual ActionResult Index(string workflow, HojaDeRutaYerbateraDto orden, DatosUsuario datosUsuario)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (datosUsuario.CentroId == 0)
            {
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            if (!Validar(orden, datosUsuario))
            {
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            if (ModelState.IsValid)
            {
                var response = ValidarNumeroHojaDeRutaYerbatera(orden, datosUsuario.CentroId, workflow);
                if (!response.Valida)
                {
                    ModelState.AddModelError("NroHojaDeRutaYerbatera", response.Error);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (workflows.ObtenerWorkflowPorPatente(orden.Patente) != null)
                {
                    ModelState.AddModelError("", Textos.OrdenCargaInterna_PatenteEnOtroWorkflow);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId,
                                                                 orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                orden.FechaEmision = DateTime.Now;
                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
                orden.Patente = orden.Patente != null ? orden.Patente.ToUpper() : "";
                orden.PatenteAcoplado = orden.PatenteAcoplado != null ? orden.PatenteAcoplado.ToUpper() : "";

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                log.Debug("CargarHojaDeRutaYerbatera: Iniciando carga de workflow para el documento: " + orden.ToJson());
                var controlRecorrido = GenerarControlRecorrido(datosUsuario);
                var resultadoActividad =
                    servicioWf.CargarHojaDeRutaYerbatera(orden, datosUsuario.CentroId, workflow,
                                                         workflowDefinicionId, datosUsuario.NombreUsuario,
                                                         controlRecorrido) as ResultadoCrearWorkflow;
                if (resultadoActividad.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoActividad);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }
                return RedirectToAction("Index", "ListaDeCamiones", new {id = resultadoActividad.InstanciaWorkflowId});
            }
            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(orden);
        }

        protected virtual void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.BocasDestino = new List<SelectListItem>();
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
        }

        protected virtual HojaDeRutaYerbateraValidaResponseDto ValidarNumeroHojaDeRutaYerbatera(HojaDeRutaYerbateraDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroHojaDeRutaYerbateraValido(orden.NroHojaDeRutaYerbatera, centroId, workflowCodigo);
        }

        protected virtual bool Validar(HojaDeRutaYerbateraDto orden, DatosUsuario usuario)
        {
            return true;
        }

        [DatosUsuario]
        protected virtual ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto { Actividad = Textos.ActCargarHojaDeRutaYerbatera, ActividadXaml = "CargarHojaDeRutaYerbatera", PuestoDeTrabajoId = usuario.PuestoDeTrabajoId, NombreUsuario = usuario.NombreUsuario };
        }
    }
}