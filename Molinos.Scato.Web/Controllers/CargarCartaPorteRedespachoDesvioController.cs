using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadCargarCartaPorteRedespachoDesvio)]
    public class CargarCartaPorteRedespachoDesvioController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarCartaPorteRedespachoDesvioService> factory;
        private readonly IFirmaProvider configuracion;

        public CargarCartaPorteRedespachoDesvioController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteRedespachoDesvioService> factory, IServicioComandos servicioComandos, IFirmaProvider configuracion)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.factory = factory;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public virtual ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var cartaPorte = servicio.ObtenerCartaPortePorInstanceId(id);
            if (cartaPorte == null)
            {
                TempData["Alerta"] = Textos.EgresoPorDesvioError;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            SetearVista(id);
            var molinos = servicio.ObtenerProveedorPorCodigoSap(configuracion.ObtenerFirmaSinLogo().CodigoSAP);
            var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
            cartaPorte.FechaEmision = DateTime.Now;
            cartaPorte.RtteComercial = cartaPorte.TitularCartaPorte;
            cartaPorte.RtteComercialId = cartaPorte.TitularCartaPorteId;
            cartaPorte.RtteComercialCodigoSap = cartaPorte.TitularCartaPorteCodigoSap;
            cartaPorte.RtteComercialCuit = cartaPorte.TitularCartaPorteCuil;
            cartaPorte.TitularCartaPorte = molinos.RazonSocial;
            cartaPorte.TitularCartaPorteCodigoSap = molinos.CodigoSap;
            cartaPorte.TitularCartaPorteCuil = molinos.Cuil;
            cartaPorte.TitularCartaPorteId = molinos.Id;
            cartaPorte.Entregador = molinos.RazonSocial;
            cartaPorte.EntregadorCuit = molinos.Cuil;
            cartaPorte.EntregadorId = molinos.Id;
            cartaPorte.Destinatario = molinos.RazonSocial;
            cartaPorte.DestinatarioCodigoSap = molinos.CodigoSap;
            cartaPorte.DestinatarioCuil = molinos.Cuil;
            cartaPorte.DestinatarioId = molinos.Id;
            cartaPorte.Destino = null;
            cartaPorte.DestinoCodigoSap = null;
            cartaPorte.DestinoCuit = null;
            cartaPorte.DestinoId = 0;
            cartaPorte.TipoComercial = null;
            cartaPorte.TipoComercialId = 0;
            cartaPorte.NroCartaPorte = null;
            cartaPorte.CEE = null;
            cartaPorte.CTG = null;
            cartaPorte.KmRecorrer = null;
            cartaPorte.TarifaReferencia = null;
            cartaPorte.TarifaTonelada = null;
            cartaPorte.Procedencia = centro.LocalidadDesc;
            cartaPorte.ProcedenciaId = centro.LocalidadId ?? 0;
            cartaPorte.ProcedenciaCodigoSap = centro.LocalidadCodigoSap;
            cartaPorte.CodEstab = centro.CodigoEstablecimiento;
            cartaPorte.TipoDeWorkflow = TipoDeWorkflow.Egreso;
            cartaPorte.InstanciaWorkflow = id;
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            cartaPorte.Vehiculos = new List<VehiculoDto> { recorrido.Vehiculo };
            cartaPorte.Workflow = recorrido.Workflow.Codigo;
            cartaPorte.NroCartaPorteOrigen = recorrido.NumeroDocumentoIngresoRelacionado;
            cartaPorte.FechaCP = DateTime.Now;

            return View(cartaPorte);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public virtual ActionResult Index(CartaPorteDto orden, DatosUsuario datosUsuario)
        {
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var vehiculos = orden.Vehiculos;
            if (datosUsuario.CentroId == 0)
            {
                log.Debug("El usuario no tiene centro elegido");
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.Remove("Id"); //Para descartar que id como guid
            if (ModelState.IsValid && vehiculos != null && vehiculos.Count != 0)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(orden.Workflow);
                if (workflowObj == null)
                {
                    log.Debug("No existe el workflow " + orden.Workflow);
                    TempData["Alerta"] = string.Format(Textos.EgresoPorDesvioNoExisteDesvioError, orden.Workflow);
                    TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                    return RedirectToAction("Index", "ListaDeCamiones");
                }

                var response = ValidarNumeroCartaPorte(orden, datosUsuario.CentroId, workflowObj.Codigo);
                if (!response.Valida)
                {
                    log.Debug("Número de carta de porte inválido");
                    ModelState.AddModelError("NroCartaPorte", response.Error);
                    SetearVista(orden.InstanciaWorkflow);
                    return View(orden);
                }

                var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
                if (otroRecorridoDelChofer != null)
                {
                    ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                    SetearVista(orden.InstanciaWorkflow);
                    return View(orden);
                }

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflowObj.Codigo);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var resultadoChofer = SetearChofer(orden.Chofer);

                if (resultadoChofer == false)
                {
                    SetearVista(orden.InstanciaWorkflow);
                    return View(orden);
                }

                orden.TransportistaId = orden.TransportistaId ?? 0;
                orden.Vehiculos = vehiculos;
                orden.FechaEmision = DateTime.Now;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }
                log.Info("CargarCartaPorteRedespachoDesvio: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActCargarCartaPorteRedespachoDesvio,
                        ActividadXaml = "CargarCartaPorteRedespachoDesvio",
                        WorkflowInstanceId = orden.InstanciaWorkflow,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };
                foreach (var vehiculo in vehiculos)
                {
                    var resultadoActividad = servicioWf.CargarCartaPorteRedespachoDesvio(orden.InstanciaWorkflow, orden, vehiculo, controlRecorrido);
                    if (resultadoActividad.HayErrores)
                    {
                        log.Debug("El servicio de workflows devolvio un error al crear la carta de porte para el workflow con patente {0}", vehiculo.Patente);
                        ModelState.AgregarErrores(resultadoActividad);
                        SetearVista(orden.InstanciaWorkflow);
                        return View(orden);
                    }
                    log.Info("CargarCartaPorteRedespachoDesvio para workflow {0} creado con éxito", workflowObj.Codigo);
                }
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            if (vehiculos == null || vehiculos.Count == 0)
            {
                log.Debug("No hay vehículos asociados a la carta de porte");
                ModelState.AddModelError("Vehiculo", string.Format(Textos.Error_Requerido, Textos.Vagones));
            }
            log.Debug("Model State Valido? {0}", ModelState.IsValid);
            if (log.IsDebugEnabled && !ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(value => value.Errors))
                {
                    log.Debug(error.ErrorMessage);
                }
            }
            SetearVista(orden.InstanciaWorkflow);
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
            var js = new DataContractJsonSerializer(typeof(CartaPorteDto));
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(model));
            var modelo = js.ReadObject(ms) as CartaPorteDto;
            return View("~/Views/CargarCartaPorte/Camion.cshtml", modelo);
        }

        public ActionResult MostrarVagonesJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorte/Camion.cshtml");
            }
            var js = new DataContractJsonSerializer(typeof(CartaPorteDto));
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(model));
            var modelo = js.ReadObject(ms) as CartaPorteDto;
            return View("~/Views/CargarCartaPorte/Vagon.cshtml", modelo);
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorte(string numero, string workflow, DatosUsuario datosUsuario)
        {
            try
            {
                log.Debug("Obteniendo carta de porte nro {0} workflow {1}", numero, workflow);
                var cartaPorteResponse = servicio.ObtenerCartaPorteAReutilizarPorNumero(numero, datosUsuario.CentroId, workflow);

                log.Debug(cartaPorteResponse.CodigoDeError == 1 ? "No se encontró la carta de porte {0}" : "Devolviendo carta de porte {0}", numero);

                return Json(new { cartaPorteResponse.CartaPorte, cartaPorteResponse.CodigoDeError, Error = cartaPorteResponse.Error }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte nro {0}", numero);
                throw;
            }
        }

        protected virtual void SetearVista(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var materiales = servicio.ListarMaterialesPorWorkflow(recorrido.Workflow.Id, recorrido.Centro.Id);
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(recorrido.Workflow.Codigo);

            ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString()).ToList();
            ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            ViewBag.BocasDestino = new List<SelectListItem>();
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDescripcion = recorrido.Workflow.Descripcion;
            ViewBag.EsIngreso = false;
            ViewBag.DeshabilitarTitular = true;
            ViewBag.DeshabilitarDestinatario = false;
            ViewBag.DeshabilitarEntregador = false;
            ViewBag.DeshabilitarTransportista = true;
            ViewBag.DeshabilitarPatentes = true;
            ViewBag.VaciarFechaVto = true;
        }

        protected virtual CartaPorteValidaResponseDto ValidarNumeroCartaPorte(CartaPorteDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroCartaPorteValidoRedespacho(orden.NroCartaPorte, centroId, workflowCodigo);
        }
    }
}