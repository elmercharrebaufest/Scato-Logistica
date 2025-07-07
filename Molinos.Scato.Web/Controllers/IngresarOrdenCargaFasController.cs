using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
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
                return View(new OrdenCargaFasDto { MaterialId = cupo.MaterialId.GetValueOrDefault(), PatenteCamion = cupo.Patente, MaterialDesc = cupo.MaterialDescripcion });
            }
            return View(new OrdenCargaFasDto());
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaFasDto orden, DatosUsuario datosUsuario, string MotivoDemora, int? Material)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            ConsultarPagoTasaMunicipal(datosUsuario.CentroId, orden.PatenteCamion, orden.PatenteAcoplado, null, orden.TipoVehiculo, string.Empty, orden.MaterialId);

            Validar(orden, workflowObj, datosUsuario);

            if (!ModelState.IsValid)
            {
                SetearVista(workflowObj);
                ViewBag.AceptaPendiente = true;
                return View(orden);
            }

            if (orden.VehiculoDemorado)
            {
                var res = Demorado(orden, MotivoDemora, Material, workflowObj, datosUsuario);
                if (res.Errores.ContainsKey("0"))
                {
                    TempData["Alerta"] = string.Format(Textos.OrdenCargaFAS_YaUsada, orden.NumeroOrden);
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    SetearVista(workflowObj);
                    ViewBag.AceptaPendiente = true;
                    return View(orden);
                }
                if (res.HayErrores)
                {
                    SetearVista(workflowObj);
                    var ordenDemorada = new OrdenCargaFasDto
                    {
                        Chofer = orden.Chofer.Cuil == "99-99999999-9" ? new ChoferDto() : orden.Chofer,
                        PatenteCamion = orden.PatenteCamion,
                        PatenteAcoplado = orden.PatenteAcoplado
                    };
                    ViewBag.AceptaPendiente = true;
                    return View(ordenDemorada);
                }
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            if (servicio.ExisteOrdenCargaFas(orden.NumeroOrden))
            {
                TempData["Alerta"] = string.Format(Textos.OrdenCargaFAS_YaUsada, orden.NumeroOrden);
                TempData["TipoAlerta"] = TipoAlerta.Error;
                SetearVista(workflowObj);
                ViewBag.AceptaPendiente = true;
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
                ViewBag.AceptaPendiente = true;
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, false);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                SetearVista(workflowObj);
                ViewBag.AceptaPendiente = true;
                return View(orden);
            }

            if (orden.DerivadoGranarioHabilitado && workflow != "1029-EgresoPorExportacionFCA" && !(orden.VehiculoDemorado || orden.Rechazado))
            {
                var domicilio = orden.TipoYOrdenDestino.Split('-');
                orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
                orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
                var dominios = new List<string> { orden.PatenteCamion };
                if (!string.IsNullOrEmpty(orden.PatenteAcoplado))
                {
                    dominios.Add(orden.PatenteAcoplado);
                }
                ConfirmarCTGVencidos(datosUsuario.CentroId);
                var resultadoAltaDummy = servicioComandos.Ejecutar(new AutorizarCpeDGDummy
                {
                    TipoVehiculo = orden.TipoVehiculo,
                    CentroId = datosUsuario.CentroId,
                    MaterialId = orden.MaterialId,
                    DestinoPlanta = orden.PlantaDGDestino ?? 0,
                    DestinoDomicilioTipo = orden.TipoDomicilioDestino ?? 0,
                    DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
                    TransportistaId = orden.TransportistaId,
                    Dominios = dominios.ToArray(),
                    KmRecorrer = !string.IsNullOrEmpty(orden.KmARecorrer) ? int.Parse(orden.KmARecorrer) : 0,
                    ChoferCuit = orden.Chofer.Cuil,
                    PagadorFleteId = orden.PagadorFleteId ?? 0,
                    CorredorId = orden.CorredorId,
                    RemitenteId = orden.RemitenteId,
                    DestinoId = orden.ClienteId,
                    ComisionistaId = orden.ComisionistaId,
                    IntermediarioFleteId = orden.IntermediarioFleteId,
                    DestinatarioId = orden.DestinatarioId,
                    AplicaDestinatario = true
                }) as ResultadoCartaPorteElectronicaDummy;
                if (resultadoAltaDummy.HayErrores)
                {
                    SetearVista(workflowObj);
                    ViewBag.ErrorAfip = resultadoAltaDummy.Errores.Values.First();
                    return View(orden);
                }

                var resultadoAnulacionDummy = servicioComandos.Ejecutar(new AnularCPEDGDummy
                {
                    CentroId = datosUsuario.CentroId,
                    NroOrden = (int)resultadoAltaDummy.NroOrden,
                    Sucursal = resultadoAltaDummy.Sucursal,
                    TipoCPE = (short)resultadoAltaDummy.TipoCPE,
                });
                if (resultadoAnulacionDummy.HayErrores)
                {
                    SetearVista(workflowObj);
                    ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                    return View(orden);
                }
            }

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenDeCargaFas,
                ActividadXaml = "IngresarOrdenCargaFas",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : string.Empty
            };

            var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaFas(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, orden.ValidaCompliance, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
            
            if (!resultadoActividad.HayErrores)
            {
                if (ResultadoPagoTasaMunicipal != null && ResultadoPagoTasaMunicipal.IdPago != 0)
                    ActualizarTasaMunicipal(resultadoActividad.InstanciaWorkflowId, ResultadoPagoTasaMunicipal.IdPago, ResultadoPagoTasaMunicipal.IdDiferenciaDePago ?? 0);

                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
            }

            ModelState.AgregarErrores(resultadoActividad);

            SetearVista(workflowObj);
            ViewBag.AceptaPendiente = true;
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
            controller.ViewBag.MaterialesDerivadoGranario = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();
        }

        [DatosUsuario]
        public JsonResult ObtenerDatos(string numero, string workflow, DatosUsuario datosUsuario)
        {
           
       
            string centro = servicio.ObtenerCentro(datosUsuario.CentroId).CodigoSAP;
            string patente = numero.ToUpper();
            OrdenDeCargaSapDto orden = new OrdenDeCargaSapDto(centro, patente , workflow);

            try
            {
                var respuestaSap =servicioComandos.Ejecutar(new ConsultarOrdenCargaFas { Orden = orden}) as ResultadoConsultaOrdenCargaFas;

                if (respuestaSap.HayErrores)
                {
                    log.Error("Error al obtener la orden de carga de SAP");
                    if(respuestaSap.Orden != null)
                    return Json(new { datosSap = respuestaSap.Orden, error = respuestaSap.Errores.First().Value}, JsonRequestBehavior.AllowGet);
                    else
                    return Json(new { datosSap = -1, error = respuestaSap.Errores.First().Value}, JsonRequestBehavior.AllowGet);
                }

                if (respuestaSap.Orden.Count > 0)
                {
                    var ordenSap = respuestaSap.Orden.First();
                
                    return Json(new { datosSap = respuestaSap.Orden }, JsonRequestBehavior.AllowGet);
                }
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

        [DatosUsuario]
        public Resultado Demorado(OrdenCargaFasDto orden, string MotivoDemora, int? Material, WorkflowDto workflowObj, DatosUsuario datosUsuario)
        {
            var resultado = new Resultado();
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

        // Utilizar método sólo para pruebas locales
        private ConsultaOrdenDeCargaResponse1 ObtenerDatosDePruebaDeSAP()
        {
            var salida = new ZSDES0300
            {
                NRO_DOC_CHOFER = "20-14692893-6",
                TIPO_DOC_CHOFER = TipoDocumentoChofer.Cuit,
                CUIT_TR = "20-20686662-5",
                KUNDE = "9950085862",
                MATNR = "99704",
                KUNNR = "7151840000",
                TIPO_COMERCIAL = "CYO",
                PATEN = "AAL001",
                ACOPL = "AAL101",
                CUIT = "27000000014",
                SOLIC = "MUNICIPALIDAD DE AVELLANEDA",
                VBELN = "0066814054", //"0099814054"
                FLETEPROPIO = string.Empty,
                CODPLANTA = "1809",
                TIPODOM = "1",
                ORDENDOM = "1",
                PAGADOR_FLETE = "7153750000",
                INHABILITADO = "X",
                CORRE = "20007126671",
                CUIT_CTA_ORDEN = "27000000014",
                CUIT_DESTINATARIO = "30500858628",
                PROV_INT_FLETE = "20686662"
                //TIPO_REVENTA = "C",
            };
            var consultaOrden = new ConsultaOrdenDeCargaResponse
            {
                Salida = new ZSDES0300[] { salida }
            };
            var orden = new ConsultaOrdenDeCargaResponse1
            {
                ConsultaOrdenDeCargaResponse = consultaOrden
            };
            return orden;
        }

        private void Validar(OrdenCargaFasDto orden, WorkflowDto workflow, DatosUsuario datosUsuario)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario && workflow.Codigo != Constantes.WorkFlow.workflowExportacionFCA;

            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);

            if (ResultadoPagoTasaMunicipal != null && !ResultadoPagoTasaMunicipal.EjecutaWorkFlow)
            {
                ModelState.AddModelError("ErrorTasaMunicipal", ResultadoPagoTasaMunicipal.MensajeAlerta);
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && orden.Inhabilitado)
            {
                ModelState.AddModelError("ClienteDesc", "El cliente está inhabilitado.");
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && material.EsDerivadoGranario && workflow.Codigo != Constantes.WorkFlow.workflowExportacionFCA  && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && material.EsDerivadoGranario && workflow.Codigo != Constantes.WorkFlow.workflowExportacionFCA && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
            {
                ModelState.AddModelError("TipoYOrdenDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && material.EsDerivadoGranario && workflow.Codigo != Constantes.WorkFlow.workflowExportacionFCA && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0))
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && string.IsNullOrEmpty(orden.NumeroOrden))
            {
                ModelState.AddModelError("NumeroOrden", string.Format(Textos.Error_Requerido, Textos.OrdenCargaFAS_OrdenCargaFas));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && orden.TransportistaId <= 0)
            {
                ModelState.AddModelError("TransportistaDesc", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && orden.ClienteId <= 0)
            {
                ModelState.AddModelError("ClienteDesc", string.Format(Textos.Error_Requerido, Textos.Cliente));
            }

            if (!(orden.Rechazado || orden.VehiculoDemorado) && material.EsDerivadoGranario && workflow.Codigo != Constantes.WorkFlow.workflowExportacionFCA && (!orden.DestinatarioId.HasValue || orden.DestinatarioId <= 0))
            {
                ModelState.AddModelError("DestinatarioDesc", string.Format(Textos.Error_Requerido, Textos.Destinatario));
            }
	    
            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("ChoferDesc", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
            }

            if(!servicio.ListarMaterialesPorWorkflow(workflow.Id, datosUsuario.CentroId).Where(x => x.MaterialId == orden.MaterialId).Any())
            {
                ModelState.AddModelError("MaterialDesc", Textos.MaterialPorWorkflow_NoExistente);
            }
        }

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
        }
    }
}