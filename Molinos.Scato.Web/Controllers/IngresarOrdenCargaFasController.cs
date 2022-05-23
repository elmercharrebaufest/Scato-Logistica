using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarOrdenCargaInterna)]
    public class IngresarOrdenCargaFasController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresarOrdenCargaFasService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly ZSDWS_SCATO servicioSap;

        public IngresarOrdenCargaFasController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresarOrdenCargaFasService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, ZSDWS_SCATO servicioSap)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
        }

        [DatosUsuario]
        public ActionResult Index(string workflow, DatosUsuario datosUsuario, int cargaDeCupoId = 0)
        {
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            SetearVista(workflowObj);
            ViewBag.AceptaPendiente = true;

            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                return View(new OrdenCargaFasDto { MaterialId = cupo.MaterialId, PatenteCamion = cupo.Patente, MaterialDesc = cupo.MaterialDescripcion });
            }
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaFasDto orden, DatosUsuario datosUsuario, string MotivoDemora, int? Material)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);

            if (orden.VehiculoDemorado)
            {
                var res = Demorado(orden, MotivoDemora, Material, workflowObj, datosUsuario);
                if (res.Errores.ContainsKey("0"))
                {
                    TempData["Alerta"] = string.Format(Textos.OrdenCargaFAS_YaUsada, orden.NumeroOrden);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    SetearVista(workflowObj);
                    return View(orden);
                }
                if (res.HayErrores)
                {
                    SetearVista(workflowObj);
                    ViewBag.AceptaPendiente = true;
                    var ordenDemorada = new OrdenCargaFasDto
                    {
                        Chofer = orden.Chofer.Cuil == "99-99999999-9" ? new ChoferDto() : orden.Chofer,
                        PatenteCamion = orden.PatenteCamion,
                        PatenteAcoplado = orden.PatenteAcoplado
                    };
                    return View(ordenDemorada);
                }
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            if (ModelState.IsValid)
            {
                if (servicio.ExisteOrdenCargaFas(orden.NumeroOrden))
                {
                    TempData["Alerta"] = string.Format(Textos.OrdenCargaFAS_YaUsada, orden.NumeroOrden);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    SetearVista(workflowObj);
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
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, false);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    SetearVista(workflowObj);
                    return View(orden);
                }

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresarOrdenDeCargaFas,
                    ActividadXaml = "IngresarOrdenCargaFas",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

                if (!Validar(orden, datosUsuario))
                {
                    SetearVista(workflowObj);
                    return View(orden);
                }

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var resultadoActividad = servicioWf.IngresarOrdenCargaFas(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, orden.ValidaCompliance, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                if (!resultadoActividad.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
                }

                ModelState.AgregarErrores(resultadoActividad);
                SetearVista(workflowObj);
                return View(orden);
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
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());

            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, workflow.CentroId);
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
        }

        [DatosUsuario]
        public JsonResult ObtenerDatos(string numero, string workflow, DatosUsuario datosUsuario)
        {
            log.Info("Empieza el método FAS");
            var consultaOrdenDeCarga = new ConsultaOrdenDeCarga
            {
                Centro = servicio.ObtenerCentro(datosUsuario.CentroId).CodigoSAP,
                Patente = numero.ToUpper()
            };
            log.Info("Termina ObetenerCentro()/ Centro: " + consultaOrdenDeCarga.Centro + " Patente: " + consultaOrdenDeCarga.Patente);

            var datosRequest = new ConsultaOrdenDeCargaRequest
            {
                ConsultaOrdenDeCarga = consultaOrdenDeCarga
            };
            log.Info("Crea Request/ Request Centro " + datosRequest.ConsultaOrdenDeCarga.Centro + " Request Patente: " + datosRequest.ConsultaOrdenDeCarga.Patente);
            try
            {
                log.Info("Empieza la llamada a SAP: Consultar Orden de Carga");
                var respuestaConsultaOrdenCarga = servicioSap.ConsultaOrdenDeCarga(datosRequest);

                log.Info("Respuesta: " + respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.ToXml());
                var datosSap = new List<OrdenCargaFasDto>();
                if (respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Any())
                {
                    log.Info("Hay al menos un item en la respuesta");
                    var ordenCargaFas = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida;
                    int count = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Count();

                    for (int i = 0; i < count; i++)
                    {
                        var numeroDocumentoChofer = ordenCargaFas[i].NRO_DOC_CHOFER;
                        if (ordenCargaFas[i].TIPO_DOC_CHOFER == TipoDocumentoChofer.Cuit && !string.IsNullOrEmpty(numeroDocumentoChofer))
                        {
                            if (numeroDocumentoChofer.Contains("-"))
                            {
                                var startPos = numeroDocumentoChofer.IndexOf("-");
                                var endPos = numeroDocumentoChofer.LastIndexOf("-");
                                numeroDocumentoChofer = numeroDocumentoChofer.Substring(startPos + 1, endPos - startPos - 1);
                            }
                            else if (numeroDocumentoChofer.Length >= 10)
                            {
                                numeroDocumentoChofer = numeroDocumentoChofer.Substring(2, 8);
                            }
                        }

                        var transportista = servicio.ObtenerProveedorPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_TR), new TiposProveedor { PR = true });
                        var proveedor = servicio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].KUNDE.TrimStart(new[] { '0' }));
                        var material = servicio.ObtenerMaterialPorCodigoSap(ordenCargaFas[i].MATNR.TrimStart(new[] { '0' }));
                        var cliente = servicio.ObtenerClientePorCodigoSap(ordenCargaFas[i].KUNAG);
                        var chofer = servicio.ObtenerChoferPorNumeroDocumento(numeroDocumentoChofer);
                        var tipoComercial = servicio.ObtenerTipoComercialPorCodigoSap(ordenCargaFas[i].TIPO_COMERCIAL);

                        if (material == null)
                        {
                            return Json(new { datosSap = -1, error = string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR) }, JsonRequestBehavior.AllowGet);
                        }
                        if (proveedor == null)
                        {
                            return Json(new { datosSap = -1, error = string.Format(Textos.OrdenCargaFAS_ProveedorInexistente, ordenCargaFas[i].KUNDE) }, JsonRequestBehavior.AllowGet);
                        }
                        if (cliente == null)
                        {
                            return Json(new { datosSap = -1, error = string.Format(Textos.OrdenCargaFAS_ClienteInexistente, ordenCargaFas[i].KUNAG) }, JsonRequestBehavior.AllowGet);
                        }
                        if (workflow.Contains("Venta") && tipoComercial.Descripcion.ToLower().Contains("EFC"))
                        {
                            log.Debug($"{ordenCargaFas[i].VBELN} no es Venta fas y tiene tipo comercial {ordenCargaFas[i].TIPO_COMERCIAL}");
                            continue;
                        }
                        if (workflow.Contains("Expo") && !tipoComercial.Descripcion.ToLower().Contains("EFC"))
                        {
                            log.Debug($"{ordenCargaFas[i].VBELN} no es Expo fas y tiene tipo comercial {ordenCargaFas[i].TIPO_COMERCIAL}");
                            continue;
                        }
                        var itemSap = new OrdenCargaFasDto
                        {
                            CuitTransporte = ConvertirCuil(ordenCargaFas[i].CUIT_TR),
                            MaterialId = material.Id,
                            MaterialDesc = material.Descripcion,
                            PatenteCamion = ordenCargaFas[i].PATEN,
                            TransportistaId = transportista != null ? transportista.Id : proveedor.Id,
                            TransportistaDesc = transportista != null ? transportista.RazonSocial : proveedor.RazonSocial,
                            PatenteAcoplado = ordenCargaFas[i].ACOPL,
                            ClienteId = cliente.Id,
                            ClienteDesc = ordenCargaFas[i].SOLIC,
                            NumeroOrden = ordenCargaFas[i].VBELN,
                            ValidaCompliance = (ordenCargaFas[i].FLETEPROPIO != string.Empty),
                            Chofer = chofer,
                            TipoComercialDesc = tipoComercial != null ? tipoComercial.Descripcion : null,
                            TipoComercialId = tipoComercial != null ? (int)(tipoComercial.Id != null ? tipoComercial.Id : 0) : 0
                        };

                        datosSap.Add(itemSap);
                    }
                    if (datosSap.Count > 0)
                    {
                        return Json(new { datosSap }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { datosSap = -1, error = Textos.OrdenCargaFAS_Inexistente + "para " + workflow }, JsonRequestBehavior.AllowGet);
                    }
                }
                log.Info("No hay items en la respuesta");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Ocurrió un error al obtener la orden de descarga");
                return Json(new { datosSap = -1, error = Textos.OrdenCargaFas_Error }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { datosSap = -1, error = Textos.OrdenCargaFAS_Inexistente }, JsonRequestBehavior.AllowGet);
        }

        private string ConvertirCuil(string cuil)
        {
            if (String.IsNullOrEmpty(cuil))
            {
                return "";
            }
            string validador1 = cuil.Substring(0, 2);
            string documento = cuil.Substring(2, 8);
            string validador2 = cuil.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }

        protected virtual bool Validar(OrdenCargaFasDto orden, DatosUsuario usuario)
        {
            return true;
        }

        [DatosUsuario]
        public Resultado Demorado(OrdenCargaFasDto orden, string MotivoDemora, int? Material, WorkflowDto workflowObj, DatosUsuario datosUsuario)
        {
            var resultado = new Resultado();
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
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
                resultado.Error("0", Textos.OrdenCargaInterna_PatenteEnOtroWorkflow);
                return resultado;
            }
            if (orden.Chofer != null && orden.Chofer.Cuil != "99-99999999-9")
            {
                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    resultado.Error("1", "ErrorChofer");
                    return resultado;
                }
            }
            else
            {
                orden.Chofer = new ChoferDto();
            }

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenDeCargaFas,
                ActividadXaml = "IngresarOrdenCargaFas",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Comentario = "Vehiculo Demorado"
            };
            orden.MaterialId = orden.VehiculoDemorado ? Material.Value : orden.MaterialId;
            orden.MotivoDemora = MotivoDemora;
            orden.VehiculoDemorado = true;
            var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflowObj.Codigo);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaFas(orden, datosUsuario.CentroId, workflowObj.Codigo, workflowDefinicionId, orden.ValidaCompliance, datosUsuario.NombreUsuario, controlRecorrido);
            if (!resultadoActividad.HayErrores)
            {
                return resultado;
            }

            return resultadoActividad;
        }
    }
}