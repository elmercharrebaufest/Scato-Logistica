using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(new[] { PermisosScato.AbmModificarDocumentoDeIngreso, PermisosScato.AbmConsultarDocumentoDeIngreso })]
    public class CamionDemoradoController : DocumentoIngresoController
    {
        private IListaDeWorkflows ListaDeWorkflows;
        private readonly IServicioActividadFactory<ICamionDemoradoService> actividadFactory;
        private readonly ZSDWS_SCATO servicioSap;

        public CamionDemoradoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, 
            IListaDeWorkflows listaDeWorkflows, IServicioActividadFactory<ICamionDemoradoService> actividadFactory,
            ZSDWS_SCATO servicioSap)
            : base(log, servicio, servicioComandos)
        {
            ListaDeWorkflows = listaDeWorkflows;
            this.actividadFactory = actividadFactory;
            this.servicioSap = servicioSap;
        }
        
        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.WorkflowInstanceUid = id;

            if (recorrido != null)
            {
                ViewBag.Rechazado = recorrido.Rechazado;
                ViewBag.Patente = recorrido.Patente;
                ViewBag.NumeroDocumento = recorrido.NumeroDocumentoIngreso;
                ViewBag.RecorridoId = recorrido.Id;
            }

            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaFas)
            {
                var orden = servicio.ObtenerOrdenCargaFasPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaFasController.SetearVista(recorrido.Workflow, servicio, this);

                var resultado = ObtenerDatos(orden.PatenteCamion, datosUsuario, orden);
                if (resultado.HayErrores)
                {
                    TempData["Alerta"] = resultado.Errores.FirstOrDefault().Value;
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return View("OrdenCargaFas", orden);
                }
                ViewBag.OrdenFas = resultado.OrdenFas.ToSelectList(f => f.NumeroOrden.ToString(), f => f.NumeroOrden);
                if (resultado.OrdenFas.Count > 1)
                {
                    return View("OrdenCargaFas", resultado.OrdenFas.FirstOrDefault());
                }
                else
                {
                    return View("OrdenCargaFas", resultado.OrdenFas.FirstOrDefault());
                }
            }
            var documentoDeIngresoAux = servicio.ObtenerCartaPorte(recorrido.Vehiculo.CartaPorteId);
            var centro = servicio.ObtenerCentro(recorrido.Centro.Id);
            documentoDeIngresoAux.TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow;
            documentoDeIngresoAux.LeerCPDeFoto = centro.LeerCPDeFoto;
            documentoDeIngresoAux.TomarFotoEnMesa = centro.TomarFotoEnMesa;
            var documentoDeIngreso = documentoDeIngresoAux;
            var foto = servicio.ObtenerFotoCPDeCartaDePortePorrecorrido(recorrido.InstanciaWorkflow);
            if (foto.Fotos.Any())
            {
                ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
            }
            
            ViewBag.DeshabilitarTitular = true;
            ViewBag.DeshabilitarEntregador = true;
            ViewBag.DeshabilitarDestinatario = true;
            ViewBag.DeshabilitarPatentes = true;
            ViewBag.AceptaRechazar = true;
            ViewBag.MotivoDemora = servicio.ObtenerMotivoDemoraRecorrido(recorrido.Id);
            CargarCartaPorteController.SetearVista(recorrido.Workflow, recorrido.Centro.Id, servicio, this);
            return View(documentoDeIngreso);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public ActionResult Index(string workflow, CartaPorteDto orden, DatosUsuario datosUsuario, int cartaPorteId, Guid id)
        {
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;
            orden.InstanciaWorkflow = id;
            orden.Id = cartaPorteId;
            ModelState.Remove("Id");
            if (ModelState.IsValid && vehiculos != null && vehiculos.Count() != 0)
            {
                var resultadoChofer = SetearChofer(orden.Chofer);
                if (!resultadoChofer)
                {
                    CargarCartaPorteController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    CargarCartaPorteController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                    return View(orden);
                }

                orden.Vehiculos = vehiculos;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }
                var resultado = servicioComandos.Ejecutar(new ModificarCartaPorte { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });

                var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
                var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.CamionDemorado,
                    ActividadXaml = "AsignacionTarjetaDeAcceso",
                    WorkflowInstanceId = id,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };
                var resultadoService = demoraService.CamionDemorado(controlRecorrido, id, false);
                if (!resultado.HayErrores && !resultadoService.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
                ModelState.AgregarErrores(resultadoService);
            }
            if (vehiculos == null || !vehiculos.Any())
            {
                ModelState.AddModelError("Vehiculo", string.Format(Textos.Error_Requerido, "Vagones"));
            }

            CargarCartaPorteController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult OrdenCargaFas(string workflow, OrdenCargaFasDto orden,Guid WorkflowId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;
            ViewBag.WorkflowInstanceUid = WorkflowId;

            ViewBag.RecorridoId = orden.RecorridoId;
            log.Debug($"Camion no granos demorado { orden.PatenteCamion }, con orden nro {orden.NumeroOrden} ({WorkflowId})");

            var workflowObjt = servicio.ObtenerWorkflowPorCodigo(workflow);
            if (ModelState.IsValid)
            {

                if (orden.PatenteCamion != null)
                {
                    orden.PatenteCamion = orden.PatenteCamion.ToUpper();
                }
                if (orden.PatenteAcoplado != null)
                {
                    orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
                }

                var resultadoChofer = SetearChofer(orden.Chofer);
                log.Debug($"({WorkflowId}) - { orden.PatenteCamion }: chofer { (resultadoChofer ? "": "no") } seteado.");

                if (!resultadoChofer)
                {
                    IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                    return View(orden);
                }
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                    return View(orden);
                }
                var resultado = servicioComandos.Ejecutar(new ModificarOrdenCargaFas { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });
                log.Debug($"({WorkflowId}) - { orden.PatenteCamion }: modificacion orden fas { orden.NumeroOrden} {(resultado.HayErrores? "con" : "sin")} error.");

                if (!resultado.HayErrores)
                {
                    var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(WorkflowId);

                    var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.CamionDemorado,
                        ActividadXaml = "AsignacionTarjetaDeAcceso",
                        WorkflowInstanceId = WorkflowId,
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };
                    var resultadoService = demoraService.CamionDemorado(controlRecorrido, WorkflowId, false);
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultado);
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                return View(orden);
            }
            IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
            log.Debug($"ErrorModel : {JsonConvert.SerializeObject(ModelState)}");
            return View(orden);
        }
        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public ActionResult Rechazar(DatosUsuario datosUsuario, Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.CamionDemorado,
                ActividadXaml = "AsignacionTarjetaDeAcceso",
                WorkflowInstanceId = id,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };
            var resultadoService = demoraService.CamionDemorado(controlRecorrido, id, true);
            if (!resultadoService.HayErrores)
            {
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            ModelState.AgregarErrores(resultadoService);
            return Json(resultadoService.Errores, JsonRequestBehavior.AllowGet);
        }
        
        private ResultadoFas ObtenerDatos(string numero, DatosUsuario datosUsuario, OrdenCargaFasDto orden )
        {
            log.Info("Empieza el método FAS");
            var resultado = new ResultadoFas();
            try
            {
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
                        var transportista = servicio.ObtenerProveedorPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_TR), new TiposProveedor { PR = true });
                        var proveedor = servicio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].KUNDE.TrimStart(new[] { '0' }));
                        var material = servicio.ObtenerMaterialPorCodigoSap(ordenCargaFas[i].MATNR.TrimStart(new[] { '0' }));
                        var cliente = servicio.ObtenerClientePorCodigoSap(ordenCargaFas[i].KUNAG);
                        var chofer = servicio.ObtenerChoferPorNumeroDocumento(ordenCargaFas[i].NRO_DOC_CHOFER);
                        var tipoComercial = servicio.ObtenerTipoComercialPorCodigoSap(ordenCargaFas[i].TIPO_COMERCIAL);

                        if (material == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR));
                        }
                        if (proveedor == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_ProveedorInexistente, ordenCargaFas[i].KUNDE));
                        }
                        if (cliente == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_ClienteInexistente, ordenCargaFas[i].KUNAG));
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
                            TipoComercialDesc = tipoComercial?.Descripcion,
                            TipoComercialId = tipoComercial != null ? (int)(tipoComercial.Id != null ? tipoComercial.Id : 0) : 0,
                            Id = orden.Id,
                            RecorridoId = orden.RecorridoId
                        };

                        datosSap.Add(itemSap);
                    }
                    resultado.OrdenFas = datosSap;
                    return resultado;
                }
                log.Info("No hay items en la respuesta");
                resultado.Error("", "No hay orden de carga");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Ocurrió un error al obtener la orden de descarga");
                resultado.Error("", "Ocurrió un error al obtener la orden de descarga");
            }
            return resultado;
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
    }
}
