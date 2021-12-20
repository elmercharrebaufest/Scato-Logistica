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
    [Autorizacion(PermisosScato.ActividadCargarCartaPorteFason)]
    public class CargarCartaPorteFasonController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarCartaPorteFasonService> factory;
        private readonly IFirmaProvider configuracion;
        private readonly IListaDeWorkflows workflows;

        public CargarCartaPorteFasonController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteFasonService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public virtual ActionResult Index(string workflow, DatosUsuario datosUsuario, string destinatarioCodigoSap = "")
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
            var carta = servicio.ObtenerCartaPorteVaciaFason(datosUsuario.CentroId, workflow, destinatarioCodigoSap, firmaCodigoSap);
            carta.EsClienteDestinatario = true;
            return View(carta);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public virtual ActionResult Index(string workflow, CartaPorteDto orden, DatosUsuario datosUsuario)
        {
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;
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

            if (ModelState.IsValid && vehiculos != null && vehiculos.Count() != 0)
            {
                var response = ValidarNumeroCartaPorte(orden, datosUsuario.CentroId, workflow);
                if (!response.Valida)
                {
                    ModelState.AddModelError("NroCartaPorte", response.Error);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (vehiculos.Any(vehiculo => workflows.ObtenerWorkflowPorPatente(vehiculo.Patente) != null))
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
                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                orden.Vehiculos = vehiculos;
                orden.FechaEmision = DateTime.Now;
                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
                orden.EsClienteDestinatario = true;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var instanceIds = new List<Guid>();

                log.Info("CargarCartaPorte: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                foreach (var vehiculo in vehiculos)
                {
                    var controlRecorrido = new ControlRecorridoDto {Actividad = Textos.ActCargarCartaPorteFason, ActividadXaml = "CargarCartaPorteFason", PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId, NombreUsuario = datosUsuario.NombreUsuario};
                    var resultadoActividad = servicioWf.CargarCartaPorteFason(orden, vehiculo, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                    if (resultadoActividad.HayErrores)
                    {
                        ModelState.AgregarErrores(resultadoActividad);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                    orden.Id = resultadoActividad.Id;
                    instanceIds.Add(resultadoActividad.InstanciaWorkflowId);
                }

                ViewBag.Headers = new Dictionary<string, string> {{"WFInstanceIds", string.Join(",", instanceIds)}};
                return RedirectToAction("Index", "ListaDeCamiones", new { id = instanceIds[0] });
            }
            if (vehiculos == null || vehiculos.Count == 0)
            {
                ModelState.AddModelError("Vehiculo", string.Format(Textos.Error_Requerido, Textos.Vagones)); 
            }

            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(orden);
        }

        public ActionResult MostrarCamion(CartaPorteDto model)
        {
            return View("~/Views/CargarCartaPorte/Camion.cshtml", model);
        }

        public ActionResult MostrarVagones(CartaPorteDto model)
        {
            return View("~/Views/CargarCartaPorte/Vagon.cshtml", model);
        }

        public ActionResult MostrarCamionJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorte/Camion.cshtml");
            }
            var modelo = model.FromJson<CartaPorteDto>();
            return View("~/Views/CargarCartaPorte/Camion.cshtml", modelo);
        }

        public ActionResult MostrarVagonesJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorte/Camion.cshtml");
            }

            var modelo = model.FromJson<CartaPorteDto>();
            return View("~/Views/CargarCartaPorte/Vagon.cshtml", modelo);
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorte(string numero, string workflow, string titularYDestinatarioCodigoSap, DatosUsuario datosUsuario)
        {
            try
            {
                log.Debug("Obteniendo carta de porte nro {0} workflow {1}", numero, workflow);
                var cartaPorteResponse = servicio.ObtenerCartaPorteAReutilizarPorNumero(numero, datosUsuario.CentroId, workflow);

                log.Debug(cartaPorteResponse.CodigoDeError == 1 ? "No se encontró la carta de porte {0}" : "Devolviendo carta de porte {0}", numero);

                return Json(new { cartaPorteResponse.CartaPorte, cartaPorteResponse.CodigoDeError, cartaPorteResponse.Error }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte nro {0}", numero);
                throw;
            }
        }

        protected virtual void SetearVista(WorkflowDto workflow, int centroId)
        {
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var ramalFerroviario = servicio.ListarRamalFerroviario();

            ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString(CultureInfo.InvariantCulture)).ToList();
            ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(CultureInfo.InvariantCulture), f => f.MaterialDesc);
            ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            ViewBag.BocasDestino = new List<SelectListItem>();
            ViewBag.Workflow = workflow.Codigo;
            ViewBag.WorkflowDescripcion = workflow.Descripcion;
            ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            ViewBag.TiposCategorias = servicio.ListarCategorias().ToSelectList(f => f.Id.ToString(),
                                                                       f => f.Clasificacion);
            ViewBag.ListaMateriales = materiales;
            ViewBag.ListaRamalFerroviario = ramalFerroviario.ToSelectList(f => f.CodigoAfip.ToString(), f => f.Descripcion);
        }

        protected virtual CartaPorteValidaResponseDto ValidarNumeroCartaPorte(CartaPorteDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroCartaPorteValido(orden.NroCartaPorte, centroId, workflowCodigo);
        }

        protected virtual bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);

            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }
            return true;
        }
    }
}