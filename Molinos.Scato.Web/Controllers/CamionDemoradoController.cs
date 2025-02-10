using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(new[] { PermisosScato.AbmModificarDocumentoDeIngreso, PermisosScato.AbmConsultarDocumentoDeIngreso })]
    public class CamionDemoradoController : DocumentoIngresoController
    {
        private IListaDeWorkflows ListaDeWorkflows;
        private readonly IServicioActividadFactory<ICamionDemoradoService> actividadFactory;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IListaDeWorkflows workflows;

        public CamionDemoradoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos,
            IListaDeWorkflows listaDeWorkflows, IServicioActividadFactory<ICamionDemoradoService> actividadFactory,
            ZSDWS_SCATO servicioSap, IListaDeWorkflows workflows)
            : base(log, servicio, servicioComandos)
        {
            ListaDeWorkflows = listaDeWorkflows;
            this.actividadFactory = actividadFactory;
            this.servicioSap = servicioSap;
            this.workflows = workflows;
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

            if (recorrido?.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaFas)
            {
                var orden = servicio.ObtenerOrdenCargaFasPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaFasController.SetearVista(recorrido.Workflow, servicio, this);
                ViewBag.workflowCodigo = recorrido.Workflow?.Codigo ?? string.Empty;

                var resultado = ObtenerDatos(orden.PatenteCamion, datosUsuario, orden);
                if (resultado.HayErrores)
                {
                    TempData["Alerta"] = resultado.Errores.FirstOrDefault().Value;
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    return View("OrdenCargaFas", orden);
                }
                ViewBag.OrdenFas = resultado.OrdenFas.ToSelectList(f => f.NumeroOrden.ToString(), f => f.NumeroOrden);
                return View("OrdenCargaFas", resultado.OrdenFas.FirstOrDefault());
            }
            else if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaInterna)
            {
                var orden = servicio.ObtenerOrdenCargaInternaPorInstanceId(recorrido.InstanciaWorkflow);
                IngresarOrdenCargaInternaController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
                return View("OrdenCargaInterna", orden);
            }
            else if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaInternaFason)
            {
                var orden = servicio.ObtenerOrdenCargaInternaFasonPorInstanceId(recorrido.InstanciaWorkflow);
                orden.DerivadoGranarioHabilitado = false;
                orden.FechaEmision = DateTime.Now;
                ViewBag.MotivoDeDemora = recorrido.MotivoDemora;
                ViewBag.WorkflowCodigo = recorrido.Workflow.Codigo;
                IngresarOrdenCargaInternaFasonController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
                return View("OrdenCargaInternaFason", orden);
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
                    ActividadXaml = "CamionDemorado",
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
        public ActionResult OrdenCargaFas(string workflow, OrdenCargaFasDto orden, Guid WorkflowId, DatosUsuario datosUsuario)
        {
            ViewBag.SoloLectura = false;
            ViewBag.RecorridoId = orden.RecorridoId;
            ViewBag.EsDocumentoDeOrigen = false;
            ViewBag.FiltrarPorTarjeta = false;
            ViewBag.Rechazado = false;
            ViewBag.WorkflowInstanceUid = WorkflowId;

            log.Debug($"Camion no granos demorado {orden.PatenteCamion}, con orden nro {orden.NumeroOrden} ({WorkflowId})");

            var workflowObjt = servicio.ObtenerWorkflowPorCodigo(workflow);

            Validar(orden);
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorAfip = Textos.OrdenCarga_ErrorValidacion;
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                return View(orden);
            }

            if (servicio.ExisteOrdenCargaFas(orden.NumeroOrden))
            {
                TempData["Alerta"] = string.Format(Textos.OrdenCargaFAS_YaUsada, orden.NumeroOrden);
                TempData["TipoAlerta"] = TipoAlerta.Error;
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
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

            var resultadoChofer = SetearChofer(orden.Chofer);
            log.Debug($"({WorkflowId}) - {orden.PatenteCamion}: chofer {(resultadoChofer ? "" : "no")} seteado.");
            if (!resultadoChofer)
            {
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, false);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                return View(orden);
            }
            log.Debug($"({WorkflowId}) - {orden.PatenteCamion}: transportista {(resultadoChofer ? "" : "no")} seteado.");

            if (orden.DerivadoGranarioHabilitado && !orden.Rechazado)
            {
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
                    AplicaDestinatario = true,
                }) as ResultadoCartaPorteElectronicaDummy;
                if (resultadoAltaDummy.HayErrores)
                {
                    IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
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
                    IngresarOrdenCargaFasController.SetearVista(workflowObjt, servicio, this);
                    ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                    return View(orden);
                }
            }

            var resultado = servicioComandos.Ejecutar(new ModificarOrdenCargaFas { Orden = orden, NombreUsuario = datosUsuario.NombreUsuario });
            log.Debug($"({WorkflowId}) - {orden.PatenteCamion}: modificacion orden fas {orden.NumeroOrden} {(resultado.HayErrores ? "con" : "sin")} error.");
            if (!resultado.HayErrores)
            {
                var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(WorkflowId);

                var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.CamionDemorado,
                    ActividadXaml = "CamionDemorado",
                    WorkflowInstanceId = WorkflowId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : string.Empty
                };
                var resultadoService = demoraService.CamionDemorado(controlRecorrido, WorkflowId, orden.Rechazado);
                if (!resultadoService.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultadoService);
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
                ActividadXaml = "CamionDemorado",
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

        [HttpPost]
        [DatosUsuario]
        public ActionResult OrdenCargaInterna(OrdenCargaInternaDto model, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorrido(model.RecorridoId);
            if (ModelState.IsValid)
            {
                if (model.DerivadoGranarioHabilitado && !model.Rechazado)
                {
                    var dominios = new List<string> { model.PatenteCamion };
                    if (!string.IsNullOrEmpty(model.PatenteAcoplado))
                    {
                        dominios.Add(model.PatenteAcoplado);
                    }
                    ConfirmarCTGVencidos(datosUsuario.CentroId);
                    var resultadoAltaDummy = servicioComandos.Ejecutar(new AutorizarCpeDGDummy
                    {
                        TipoVehiculo = model.TipoVehiculo,
                        CentroId = datosUsuario.CentroId,
                        MaterialId = model.MaterialId,
                        DestinoId = model.DestinoId,
                        DestinoPlanta = model.PlantaDGDestino ?? 0,
                        DestinoDomicilioTipo = model.TipoDomicilioDestino ?? 0,
                        DestinoDomicilioOrden = model.OrdenDomicilioDestino ?? 0,
                        TransportistaId = model.TransportistaId,
                        Dominios = dominios.ToArray(),
                        KmRecorrer = !string.IsNullOrEmpty(model.KmARecorrer) ? int.Parse(model.KmARecorrer) : 0,
                        ChoferCuit = model.Chofer.Cuil,
                        PagadorFleteId = model.PagadorFleteId ?? 0,
                    }) as ResultadoCartaPorteElectronicaDummy;
                    if (resultadoAltaDummy.HayErrores)
                    {
                        IngresarOrdenCargaInternaController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
                        ViewBag.ErrorAfip = resultadoAltaDummy.Errores.Values.First();
                        return View(model);
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
                        IngresarOrdenCargaInternaController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
                        ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                        return View(model);
                    }
                }

                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.CamionDemorado,
                    ActividadXaml = "CamionDemorado",
                    WorkflowInstanceId = recorrido.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Comentario = model.Rechazado ? $"Vehiculo Rechazado. {model.MotivoRechazo}" : string.Empty
                };
                var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
                var resultadoActividad = demoraService.CamionDemorado(controlRecorrido, recorrido.InstanciaWorkflow, model.Rechazado);
                if (!resultadoActividad.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
                ModelState.AgregarErrores(resultadoActividad);
            }
            IngresarOrdenCargaInternaController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult OrdenCargaInternaFason(OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorrido(orden.RecorridoId);
            var workflowObje = recorrido.Workflow;
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;

            if (orden.TipoYOrdenDestino != null) 
            {
                var domicilio = orden.TipoYOrdenDestino.Split('-');
                orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
                orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
            }

            // Validar solo si no es rechazo o demora
            if (!orden.Rechazado)
            {
                Validar(orden, workflowObje.Id, datosUsuario);
            }
            else
            {
                ValidarRechazo(recorrido.InstanciaWorkflow, recorrido.WorkflowDefinicionId, orden, datosUsuario);
                if (ModelState.IsValid)
                {
                    this.RechazarFason(recorrido.InstanciaWorkflow, recorrido.WorkflowDefinicionId, orden, datosUsuario);
                    return RedirectToAction("Index", "ListaDeCamiones");
                }
            }

            if (!ModelState.IsValid)
            {
                IngresarOrdenCargaInternaFasonController.SetearVista(workflowObje, datosUsuario.CentroId, servicio, this);
                ViewBag.ErrorAfip = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                ViewBag.HasErrors = true;
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

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflowObje.Codigo);
                IngresarOrdenCargaInternaFasonController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                ViewBag.HasErrors = true;
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflowObje.Codigo);
                IngresarOrdenCargaInternaFasonController.SetearVista(workflowObj, datosUsuario.CentroId, servicio, this);
                ViewBag.HasErrors = true;
                return View(orden);
            }

            if (orden.DerivadoGranarioHabilitado && !(orden.Demorado || orden.Rechazado))
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
                    DestinoId = orden.ClienteId,
                    DestinatarioId = orden.DestinatarioId,
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
                    ComisionistaId = orden.ComisionistaId,
                    IntermediarioFleteId = orden.IntermediarioFleteId,
                    AplicaDestinatario = true,
                    Observaciones = orden.Observaciones
                }) as ResultadoCartaPorteElectronicaDummy;

                if (resultadoAltaDummy.HayErrores)
                {
                    IngresarOrdenCargaInternaFasonController.SetearVista(workflowObje, datosUsuario.CentroId, servicio, this);
                    ViewBag.ErrorAfip = resultadoAltaDummy.Errores.Values.First();
                    ViewBag.HasErrors = true;
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
                    IngresarOrdenCargaInternaFasonController.SetearVista(workflowObje, datosUsuario.CentroId, servicio, this);
                    ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                    ViewBag.HasErrors = true;
                    return View(orden);
                }
            }

            var resultado = servicioComandos.Ejecutar(
                new ModificarOrdenCargaInternaFason 
                { 
                    Orden = orden, 
                    NombreWorkflow = workflowObje.Codigo, 
                    NombreUsuario = datosUsuario.NombreUsuario 
                });

            if (!resultado.HayErrores)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.CamionDemorado,
                    ActividadXaml = "CamionDemorado",
                    WorkflowInstanceId = recorrido.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : string.Empty
                };

                var demoraService = actividadFactory.CrearServicio(recorrido.WorkflowDefinicionId);
                var resultadoActividad = demoraService.CamionDemorado(controlRecorrido, recorrido.InstanciaWorkflow, orden.Rechazado);
                if (!resultadoActividad.HayErrores)
                {
                    return RedirectToAction("Index", "ListaDeCamiones");
                }

                ModelState.AgregarErrores(resultadoActividad);
            }

            IngresarOrdenCargaInternaFasonController.SetearVista(recorrido.Workflow, datosUsuario.CentroId, servicio, this);
            ViewBag.ErrorAfip = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            ViewBag.HasErrors = true;
            
            return View(orden);
        }

        private void RechazarFason(Guid InstanciaWorkflow, int WorkflowDefinicionId, OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.CamionDemorado,
                    ActividadXaml = "CamionDemorado",
                    WorkflowInstanceId = InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : string.Empty
                };
                var demoraService = actividadFactory.CrearServicio(WorkflowDefinicionId);
                var resultadoActividad = demoraService.CamionDemorado(controlRecorrido, InstanciaWorkflow, orden.Rechazado);

                if (!resultadoActividad.HayErrores)
                {
                    ModelState.Clear();
                }
                else
                {
                    ModelState.AgregarErrores(resultadoActividad);
                }
            }
        }

        protected virtual void Validar(OrdenCargaInternaFasonDto orden, int workflowId, DatosUsuario datosUsuario)
        {
            var materialesPermitidos = servicio.ListarMaterialesPorWorkflow(workflowId, datosUsuario.CentroId).Select(m => m.MaterialId).ToList();
            var esClienteProvisorio = orden.ClienteId == 0 ? false : servicio.ObtenerCliente(orden.ClienteId).EsClienteProvisorio;
            var workflowsInstancia = workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion);
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChoferDemorado(orden.Chofer.Id, workflowsInstancia.Id);

            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
            }

            if (workflowsInstancia != null && servicio.ExisteOrdenCargarFasonDemorado(orden.NumeroOrdenExterno, workflowsInstancia))
            {
                ModelState.AddModelError("1", string.Format(Textos.IdOperacionesYaUtilizado, orden.NumeroOrdenExterno));
            }

            if ((orden.Chofer == null || orden.Chofer.Cuil == null))
            {
                ModelState.AddModelError("Chofer.Cuil", string.Format(Textos.Error_Requerido, "Cuil del Chofer"));
            }

            if (orden.NumeroOrdenExterno == null)
            {
                ModelState.AddModelError("NumeroOrdenExterno", string.Format(Textos.Error_Requerido, Textos.OrdenCargaInterna_NumeroOperaciones));
            }

            if (orden.DerivadoGranarioHabilitado && (string.IsNullOrEmpty(orden.KmARecorrer) || !int.TryParse(orden.KmARecorrer, out int km) || km <= 0))
            {
                ModelState.AddModelError("KmARecorrer", string.Format(Textos.Error_Requerido, "km"));
            }

            if (orden.DerivadoGranarioHabilitado && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (orden.DerivadoGranarioHabilitado && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
            {
                ModelState.AddModelError("TipoYOrdenDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
            }

            if (orden.DerivadoGranarioHabilitado && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0))
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }

            if (esClienteProvisorio && (!orden.RemitenteId.HasValue || orden.RemitenteId == 0))
            {
                ModelState.AddModelError("Remitente", string.Format(Textos.Error_Requerido, Textos.Remitente));
            }

            if (orden.DerivadoGranarioHabilitado && string.IsNullOrEmpty(orden.Destinatario))
            {
                ModelState.AddModelError("Destinatario", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_Destinatario));
            }

            if (!materialesPermitidos.Contains(orden.MaterialId))
            {
                ModelState.AddModelError("MaterialId", Textos.MaterialNoPermitido);
            }
        }

        //TODO Refactorizar 1029-EgresoPorExportacionFCA por constante
        private ResultadoFas ObtenerDatos(string numero, DatosUsuario datosUsuario, OrdenCargaFasDto orden)
        {
            log.Info("Empieza el método FAS");
            var resultado = new ResultadoFas();
            var workflowCodigo = servicio.ObtenerRecorrido(orden.RecorridoId)?.Workflow?.Codigo;
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
                //var respuestaConsultaOrdenCarga = ObtenerDatosDePruebaDeSAP();

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
                        var chofer = servicio.ObtenerChoferPorNumeroDocumento(ordenCargaFas[i].NRO_DOC_CHOFER);
                        var tipoComercial = servicio.ObtenerTipoComercialPorCodigoSap(ordenCargaFas[i].TIPO_COMERCIAL);
                        var pagadorFlete = servicio.ObtenerClientePorCodigoSap(ordenCargaFas[i].PAGADOR_FLETE);

                        if (material == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR));
                            break;
                        }
                        if (proveedor == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_ProveedorInexistente, ordenCargaFas[i].KUNDE));
                            break;
                        }

                        if (!string.IsNullOrEmpty(ordenCargaFas[i].PAGADOR_FLETE) && pagadorFlete == null)
                        {
                            resultado.Error("", string.Format(Textos.OrdenCargaFAS_PagadorFleteInexistente, ordenCargaFas[i].PAGADOR_FLETE));
                            break;
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
                            NumeroOrden = ordenCargaFas[i].VBELN,
                            ValidaCompliance = (ordenCargaFas[i].FLETEPROPIO != string.Empty),
                            Chofer = chofer,
                            TipoComercialDesc = tipoComercial?.Descripcion,
                            TipoComercialId = tipoComercial != null ? (int)(tipoComercial.Id != null ? tipoComercial.Id : 0) : 0,
                            Id = orden.Id,
                            RecorridoId = orden.RecorridoId,
                            DerivadoGranarioHabilitado = workflowCodigo == "1029-EgresoPorExportacionFCA" ? false : material.EsDerivadoGranario,
                            PlantaDGDestino = material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && !string.IsNullOrEmpty(ordenCargaFas[i].CODPLANTA) ? int.Parse(ordenCargaFas[i].CODPLANTA) : (int?)null,
                            OrdenDomicilioDestino = material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && !string.IsNullOrEmpty(ordenCargaFas[i].ORDENDOM) ? int.Parse(ordenCargaFas[i].ORDENDOM) : (int?)null,
                            PagadorFleteId = material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" ? pagadorFlete?.Id : (int?)null,
                            PagadorFlete = material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" ? pagadorFlete?.Descripcion : null,
                            Inhabilitado = workflowCodigo == "1029-EgresoPorExportacionFCA" ? false : !string.IsNullOrEmpty(ordenCargaFas[i].INHABILITADO),
                            TipoDomicilioDestino = material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && !string.IsNullOrEmpty(ordenCargaFas[i].TIPODOM) ? int.Parse(ordenCargaFas[i].TIPODOM) : (int?)null,
                        };

                        if (material.EsDerivadoGranario
                           && workflowCodigo != "1029-EgresoPorExportacionFCA"
                           && (!string.IsNullOrEmpty(ordenCargaFas[i].TIPO_REVENTA)
                           || (string.IsNullOrEmpty(ordenCargaFas[i].TIPO_REVENTA) && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))))
                        {
                            var cliente = servicio.ListarClientesPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT)).FirstOrDefault();
                            if (cliente == null)
                            {
                                resultado.Error("", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destino, ordenCargaFas[i].CUIT));
                                break;
                            }
                            itemSap.ClienteId = cliente.Id;
                            itemSap.ClienteDesc = cliente.Descripcion;
                        }
                        else
                        {
                            var cliente = servicio.ObtenerClientePorCodigoSap(ordenCargaFas[i].KUNAG);
                            if (cliente == null)
                            {
                                resultado.Error("", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteSAP, ordenCargaFas[i].KUNAG));
                                break;
                            }
                            itemSap.ClienteId = cliente.Id;
                            itemSap.ClienteDesc = cliente.Descripcion;
                        }

                        if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && ordenCargaFas[i].TIPO_REVENTA == Constantes.SAP.TipoReventaComisionista && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))
                        {
                            var comisionista = servicio.ObtenerClientePorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_CTA_ORDEN));
                            itemSap.Comisionista = comisionista?.Descripcion;
                            itemSap.ComisionistaId = comisionista?.Id;
                        }
                        else if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && ordenCargaFas[i].TIPO_REVENTA == Constantes.SAP.TipoReventaRemitente && !string.IsNullOrEmpty(ordenCargaFas[i].CUIT_CTA_ORDEN))
                        {
                            var remitente = servicio.ObtenerClientePorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_CTA_ORDEN));
                            itemSap.Remitente = remitente?.Descripcion;
                            itemSap.RemitenteId = remitente?.Id;
                        }

                        if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && !string.IsNullOrWhiteSpace(ordenCargaFas[i].CUIT_DESTINATARIO))
                        {
                            var destinatario = servicio.ListarClientesPorCuit(ConvertirCuil(ordenCargaFas[i].CUIT_DESTINATARIO)).FirstOrDefault();

                            if (destinatario == null)
                            {
                                resultado.Error("", string.Format(Textos.OrdenCargaFAS_ClienteInexistenteCUIT, Textos.Destinatario, ordenCargaFas[i].CUIT_DESTINATARIO));
                                break;
                            }

                            itemSap.DestinatarioId = destinatario.Id;
                            itemSap.DestinatarioDesc = destinatario.Descripcion;
                        }

                        if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && string.IsNullOrEmpty(ordenCargaFas[i].CUIT_DESTINATARIO))
                        {
                            itemSap.DestinatarioId = itemSap.ClienteId;
                            itemSap.DestinatarioDesc = itemSap.ClienteDesc;
                        }

                        if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA"
                            && !string.IsNullOrEmpty(ordenCargaFas[i].CORRE)
                            && ordenCargaFas[i].CORRE != "NO POSEE")
                        {
                            var corredor = servicio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].CORRE);
                            if (corredor == null)
                            {
                                resultado.Error("", $"No se encontro un corredor con el codigo SAP {ordenCargaFas[i].CORRE}");
                                break;
                            }
                            itemSap.Corredor = corredor?.Descripcion;
                            itemSap.CorredorId = corredor?.Id;
                        }

                        if (material.EsDerivadoGranario && workflowCodigo != "1029-EgresoPorExportacionFCA" && !string.IsNullOrEmpty(ordenCargaFas[i].PROV_INT_FLETE))
                        {
                            var intermediarioFlete = servicio.ObtenerProveedorPorCodigoSap(ordenCargaFas[i].PROV_INT_FLETE.TrimStart(new[] { '0' }));
                            if (intermediarioFlete == null)
                            {
                                resultado.Error("", $"El Intermediario Flete con código {ordenCargaFas[i].PROV_INT_FLETE} no está registrado en SCATO.");
                                break;
                            }

                            itemSap.IntermediarioFleteId = intermediarioFlete.Id;
                            itemSap.IntermediarioFlete = intermediarioFlete.Descripcion;
                        }

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

        private void Validar(OrdenCargaFasDto orden)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            var workflowCodigo = servicio.ObtenerRecorrido(orden.RecorridoId)?.Workflow?.Codigo;
            orden.DerivadoGranarioHabilitado = workflowCodigo == "1029-EgresoPorExportacionFCA" ? false : material.EsDerivadoGranario;
            if (!orden.Rechazado && orden.Inhabilitado)
            {
                ModelState.AddModelError("ClienteDesc", "El cliente está inhabilitado.");
            }

            if (!orden.Rechazado && orden.DerivadoGranarioHabilitado && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (!orden.Rechazado && orden.DerivadoGranarioHabilitado && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
            {
                ModelState.AddModelError("TipoYOrdenDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
            }

            if (!orden.Rechazado && orden.DerivadoGranarioHabilitado && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0))
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }

            if (!orden.Rechazado && string.IsNullOrEmpty(orden.NumeroOrden))
            {
                ModelState.AddModelError("NumeroOrden", string.Format(Textos.Error_Requerido, Textos.OrdenCargaFAS_OrdenCargaFas));
            }

            if (!orden.Rechazado && orden.TransportistaId <= 0)
            {
                ModelState.AddModelError("TransportistaDesc", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }

            if (!orden.Rechazado && orden.ClienteId <= 0)
            {
                ModelState.AddModelError("ClienteDesc", string.Format(Textos.Error_Requerido, Textos.Cliente));
            }

            if (!orden.Rechazado && orden.DerivadoGranarioHabilitado && (!orden.DestinatarioId.HasValue || orden.DestinatarioId <= 0))
            {
                ModelState.AddModelError("DestinatarioDesc", string.Format(Textos.Error_Requerido, Textos.Destinatario));
            }
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
                KUNNR = "3815870000",
                TIPO_COMERCIAL = "CYO",
                PATEN = "ALO660",
                ACOPL = "ALO661",
                SOLIC = "MUNICIPALIDAD DE AVELLANEDA",
                VBELN = "0099814054",
                FLETEPROPIO = string.Empty,
                CODPLANTA = "1809",
                TIPODOM = "1",
                ORDENDOM = "3",
                PAGADOR_FLETE = "7153750000",
                INHABILITADO = "X",
                CORRE = "123AEA",
                CUIT_CTA_ORDEN = "27000000014",
                TIPO_REVENTA = "C",
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

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
        }

        private void ValidarRechazo(Guid InstanciaWorkflow, int WorkflowDefinicionId, OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {

            if (string.IsNullOrEmpty(orden.MotivoRechazo))
            {
                ModelState.AddModelError("MotivoRechazo", string.Format(Textos.Error_Requerido, "Motivo de rechazo"));
            }
            else if (orden.MotivoRechazo.Length < 10)
            {
                ModelState.AddModelError("MotivoRechazo", string.Format(Textos.Error_LargoMensaje, "Motivo de rechazo", "10"));
            }

            orden.PatenteCamion = ValidarPatente(orden.PatenteCamion, "PatenteCamion");
        }

        private string ValidarPatente(string patente, string fieldName)
        {
            if (string.IsNullOrEmpty(patente))
            {
                ModelState.AddModelError(fieldName, string.Format(Textos.Error_Requerido, "Patente"));
            }
            else
            {
                patente = patente.ToUpper();
            }

            return patente;
        }
    }
}