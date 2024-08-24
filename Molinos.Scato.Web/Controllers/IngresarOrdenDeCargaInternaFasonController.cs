using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
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
using System.Net;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarOrdenCargaInternaFason)]
    public class IngresarOrdenCargaInternaFasonController : DocumentoIngresoController
    {
        private readonly IServicioOperaciones servicioOperaciones;
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly ICache cache;
        private readonly ZSDWS_SCATO servicioSap;

        public IngresarOrdenCargaInternaFasonController(ILogger log, IServicioRepositorio servicio,IServicioOperaciones servicioOperaciones, IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, ICache cache, ZSDWS_SCATO servicioSap)
            : base(log, servicio, servicioComandos)
        {
            this.servicioOperaciones = servicioOperaciones;
            this.factory = factory;
            this.workflows = workflows;
            this.cache = cache;
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
            SetearVista(workflowObj, datosUsuario.CentroId);

            var numeroOrden = servicio.ObtenerNumeroDocumentoFasonGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            var orden = new OrdenCargaInternaFasonDto { FechaEmision = DateTime.Now, NumeroOrden = numeroOrden };
            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                orden.PatenteCamion = cupo.Patente;
                orden.MaterialId = cupo.MaterialId;
                orden.MaterialDesc = cupo.MaterialDescripcion;
            }
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {
            var workflowObje = servicio.ObtenerWorkflowPorCodigo(workflow);

            if (orden.TipoYOrdenDestino != null)
            {
                var domicilio = orden.TipoYOrdenDestino.Split('-');
                orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
                orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
            }

            Validar(orden, datosUsuario);

            if (!ModelState.IsValid)
            {
                SetearVista(workflowObje, datosUsuario.CentroId);
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

            if (servicio.ExisteOrdenCargaFason(orden.NumeroOrdenExterno))
            {
                TempData["Alerta"] = string.Format(Textos.IdOperacionesYaUtilizado, orden.NumeroOrdenExterno);
                TempData["TipoAlerta"] = TipoAlerta.Error;
                SetearVista(workflowObje, datosUsuario.CentroId);
                ViewBag.AceptaPendiente = true;
                return View(orden);
            }

            if (workflows.ObtenerWorkflowPorPatente(orden.PatenteCamion) != null)
            {
                TempData["Alerta"] = Textos.PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                ViewBag.HasErrors = true;
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
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
                    SetearVista(workflowObje, datosUsuario.CentroId);
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
                    SetearVista(workflowObje, datosUsuario.CentroId);
                    ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                    ViewBag.HasErrors = true;
                    return View(orden);
                }
            }

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaInternaFason,
                ActividadXaml = "IngresarOrdenCargaInternaFason",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : (orden.Demorado ? $"Vehiculo Demorado. {orden.MotivoDemora}" : string.Empty)
            };

            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaInternaFason(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;

            if (resultadoActividad != null && !resultadoActividad.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
            }

            ModelState.AgregarErrores(resultadoActividad);
            SetearVista(workflowObje, datosUsuario.CentroId);
            ViewBag.HasErrors = true;
            return View(orden);
        }

        private void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);

            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
            controller.ViewBag.WorkflowId = workflow.Id;
            controller.ViewBag.MaterialesDerivadoGranario = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();

        }

        protected static string MascaraCuit(string entrada)
        {
            if (entrada.Length != 11)
            {
                return entrada;
            }
            return entrada.Substring(0, 2) + "-" + entrada.Substring(2, 8) + "-" + entrada.Substring(10);
        }

        protected virtual void Validar(OrdenCargaInternaFasonDto orden, DatosUsuario datosUsuario)
        {
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            var esClienteProvisorio = orden.ClienteId == 0 ? false : servicio.ObtenerCliente(orden.ClienteId).EsClienteProvisorio;
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;

            if (otroRecorridoDelChofer != null)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
            }

            if (material.EsDerivadoGranario && !orden.PlantaDGDestino.HasValue)
            {
                ModelState.AddModelError("PlantaDGDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            }

            if (material.EsDerivadoGranario && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
            {
                ModelState.AddModelError("TipoYOrdenDestino", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
            }

            if (material.EsDerivadoGranario && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0))
            {
                ModelState.AddModelError("PagadorFlete", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
            }

            if (esClienteProvisorio && (!orden.ComisionistaId.HasValue || orden.ComisionistaId == 0) && (!orden.RemitenteId.HasValue || orden.RemitenteId == 0))
            {
                ModelState.AddModelError("Comisionista", string.Format(Textos.Error_Requerido, Textos.Comisionista));
                ModelState.AddModelError("Remitente", string.Format(Textos.Error_Requerido, Textos.Comisionista));
            }

            if (material.EsDerivadoGranario && string.IsNullOrEmpty(orden.Destinatario))
            {
                ModelState.AddModelError("Destinatario", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_Destinatario));
            }
        }

        [HttpGet]
        public JsonResult MostrarMensajeRecorridoAnterior(string patente)
        {
            var recorridoAnterior = servicio.RecorridoRepetidoEnElDia(patente);
            var mensajeIngresoRepetido = recorridoAnterior ? String.Format(Textos.IngresoRepetido) : string.Empty;
            return Json(new { mensaje = mensajeIngresoRepetido }, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerOrdenDeCargaOperacionesPorPatente(string patente, string workflow, DatosUsuario datosUsuario)
        {
            var response = new RespuestaEstandarDto<List<OrdenDeCargaDto>>();
            bool fleteMoa = workflow == "SLO.EgresoClienteFason";
            
            try
            {
                var restResponse = OrdenesFiltradas(ObtenerRespuestaOrdenDeCargaOperaciones(patente));
                
                if (restResponse != null)
                {
                    foreach (var item in restResponse)
                    {
                        var choferCuil = ConvertirCuil(item.CUILChofer);
                        var chofer = servicio.ObtenerChoferPorCuit(choferCuil);

                        if (chofer == null)
                        {
                            string[] partes = item.NombreChofer?.Trim()?.Split(' ');

                            string nombre = partes[0];
                            string apellido = partes[partes.Length - 1];

                            var choferNuevo = new ChoferDto
                            {
                                Nombre = nombre,
                                Apellido = apellido,
                                TipoDocumentoIdentidadId = 1,
                                Cuil = choferCuil,
                                NumeroDeDocumento = ObtenerDocumentoDesdeCuil(item.CUILChofer)
                            };

                            var resultChofer = SetearChofer(choferNuevo);

                            if (!resultChofer)
                            {
                                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se pudo ingresar el chofer " + item.NombreChofer + " del numero de orden: " + item.Id, TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                                break;

                            }
                        }

                    }

                    if (restResponse.Count == 0)
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se encontró ninguna Orden de Carga Fason con la patente ingresada", TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                    else
                        response.Data = restResponse;
                }
                else
                {
                    var cachedResponse = cache.Obtener<List<OrdenDeCargaDto>>($"Operaciones:OrdenesDeCarga");
                    cachedResponse = cachedResponse.Where(q => q.PatenteChasis == patente).ToList();
                    if (cachedResponse.Count == 0)
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se encontró ninguna Orden de Carga Fason con la patente ingresada", TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                    else
                        response.Data = cachedResponse;
                }
            }
            catch (Exception ex)
            {       
                var mensaje = $"Ocurrio un error al consultar el servicio ObtenerOrdenesDeCarga con la patente {patente}";
                log.Error(ex,mensaje);
                throw;
              //  response.Mensajes.Add(new MensajeEstandarDto { Mensaje = mensaje + " " + ex.Message, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error }); 
              //  return Json(response, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private List<OrdenDeCargaDto> OrdenesFiltradas(List<OrdenDeCargaDto> ordenDeCargaDtos)
        {
            return ordenDeCargaDtos.Where(orden => !servicio.ExisteOrdenCargaFason(orden.Id.ToString())).ToList();
        }

        [DatosUsuario]
        public JsonResult ObtenerOrdenDeCargaOperacionesSeleccionada(string clienteCUIT, string transportistaCUIT, string patente, string acoplado, string materialSAP, string ordenId,string DestinoCUIT, DatosUsuario datosUsuario)
        {
            clienteCUIT = ConvertirCuil(clienteCUIT);
            transportistaCUIT = ConvertirCuil(transportistaCUIT);
            DestinoCUIT = ConvertirCuil(DestinoCUIT);

            var response = new RespuestaEstandarDto<OrdenDeCargaComplementariaDto>();

            try
            {
                var consultaOrdenDeCarga = new ConsultaOrdenDeCarga
                {
                    Centro = servicio.ObtenerCentro(datosUsuario.CentroId).CodigoSAP,
                    Patente = patente.ToUpper()
                };

                var datosRequest = new ConsultaOrdenDeCargaRequest
                {
                    ConsultaOrdenDeCarga = consultaOrdenDeCarga
                };

                ComprobarClienteUnico(clienteCUIT);
                var cliente = servicio.ObtenerClientePorCuit(clienteCUIT);

                var destino = servicio.ObtenerClientePorCuit(DestinoCUIT);

                var transportista = servicio.ObtenerProveedorPorCuit(transportistaCUIT, new TiposProveedor { PR = true });
                var resp = ObtenerRespuestaOrdenDeCargaOperaciones(patente);
                var orden = ajustarOrdenFormatoRequerido(resp.FirstOrDefault(x => x.Id == Convert.ToInt32(ordenId)));
           

                var choferCuil = ConvertirCuil(orden.CUILChofer);
                var chofer = servicio.ObtenerChoferPorCuit(choferCuil);

                var destinatarioCuit = ConvertirCuil(DefinirDestinatario(orden));
                var destinatarioDescrip = servicio.ObtenerClientePorCuit(string.IsNullOrEmpty(destinatarioCuit) ? "" : destinatarioCuit);

                var material = servicio.ObtenerMaterialPorCodigoSap(materialSAP);


                var resultadoEscalables = servicioComandos.Ejecutar(GenerarConsultaEscalables(patente, acoplado, datosUsuario.NombreUsuario)) as ResultadoEscalables;

                if (cliente == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No se encontró un Cliente para el cuit {clienteCUIT}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (transportista == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No se encontró un Transportista para el cuit {transportistaCUIT}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (material == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No existe material con el codigo de SAP {materialSAP}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (resultadoEscalables.HayErrores)
                {
                    var ordenDeCargaComplementario = new OrdenDeCargaComplementariaDto
                    {
                        ClienteId = cliente?.Id,
                        ClienteDescripcion = cliente?.Descripcion != null ? cliente.Descripcion : "",
                        TransportistaId = transportista?.Id,
                        TransportistaDescripcion = transportista?.RazonSocial != null ? transportista.RazonSocial : "",
                        TipoDeVehiculo = (int)(resultadoEscalables.Categoria ?? TipoVehiculo.Camión),
                        MaterialId = material.Id,
                        EsDerivadoGranario = material.EsDerivadoGranario,
                        Orden = orden,
                        TieneErrorCNRT = resultadoEscalables.HayErrores,
                        DestinoId = destino?.Id,
                        DestinoDescripcion = destino?.Descripcion != null ? destino.Descripcion : "",
                    };
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"Error al obtener el tipo de vehículo por patente: {resultadoEscalables.Errores.Values.First()}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    response.Data = ordenDeCargaComplementario;
                }
                if (response.EsValido)
                {
                    var ordenDeCargaComplementario = new OrdenDeCargaComplementariaDto
                    {
                        ClienteId = cliente?.Id,
                        ClienteDescripcion = cliente?.Descripcion != null ? cliente.Descripcion : "" ,
                        TransportistaId = transportista?.Id,
                        TransportistaDescripcion = transportista?.RazonSocial != null ? transportista.RazonSocial : "" ,
                        TipoDeVehiculo = (int)(resultadoEscalables.Categoria ?? TipoVehiculo.Camión),
                        MaterialId = material.Id,
                        EsDerivadoGranario = material.EsDerivadoGranario,
                        Orden = orden,
                        TieneErrorCNRT = resultadoEscalables.HayErrores,
                        DestinoId = destino?.Id,
                        DestinoDescripcion = destino?.Descripcion != null ? destino.Descripcion : "",
                    };

                    response.Data = ordenDeCargaComplementario;
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException ex)
            {
                var mensaje = $"Ocurrio un error al consultar el servicio ObtenerOrdenesDeCarga con la patente {patente}";
                log.Error(ex, mensaje);

                var data = new
                {
                    success = false,
                    errorResponse = new
                    {
                        error = ex.Message,
                        duplicado = true
                    }
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mensaje = $"Ocurrio un error al consultar el servicio ObtenerOrdenesDeCarga con la patente {patente}";
                log.Error(ex, mensaje);

                var data = new
                {
                    success = false,
                    errorResponse = new
                    {
                        error = ex.Message,
                        duplicado = false
                    }
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }

           
        }

        private void ComprobarClienteUnico(string cuitCliente)
        {
            try
            {
                var respuesta = servicio.ContarClientes(cuitCliente);
                if (respuesta > 1) throw new InvalidOperationException("La secuencia contiene más de un elemento");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private OrdenDeCargaDto ajustarOrdenFormatoRequerido(OrdenDeCargaDto orden)
        {
            var destinatario = servicio.ObtenerClientePorCuit(ConvertirCuil(orden?.CUITDestinatario));
            var intermediario = servicio.ObtenerProveedorPorCuit(ConvertirCuil(orden?.CUITIntermediarioFlete), new TiposProveedor { PR = true });
            var destino = servicio.ObtenerClientePorCuit(ConvertirCuil(orden?.CUITDestino));
            var pagadorFlete = ConvertirCuil(orden?.PagadorFlete);

            orden.RazonSocialIntermediarioFlete = intermediario?.RazonSocial != null ? intermediario.RazonSocial : "";
            orden.RazonSocialDestinatario = destinatario?.Descripcion != null ? destinatario.Descripcion : "";
            orden.RazonSocialDestino = destino?.Descripcion != null ? destino.Descripcion : "";
            orden.PagadorFlete = pagadorFlete;
            return orden; 
        }

        public JsonResult CachearOrdenesDeCargaOperaciones()
        {
            List<OrdenDeCargaDto> restResponse = ObtenerRespuestaOrdenDeCargaOperaciones(null);
            cache.Remover($"Operaciones:OrdenesDeCarga");
            cache.Agregar($"Operaciones:OrdenesDeCarga", restResponse);

            var response = cache.Obtener<List<OrdenDeCargaDto>>($"Operaciones:OrdenesDeCarga");

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
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

        private List<OrdenDeCargaDto> ObtenerRespuestaOrdenDeCargaOperaciones(string patente)
        {
            try
            {
                IEnumerable<OrdenDeCargaDto> data = servicioOperaciones.ObtenerOrdenesDeCarga(patente);
                return data.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        private string DefinirDestinatario(OrdenDeCargaDto orden)
        {
            // Revisa si el campo Reventa está activado o no
            if (!orden.Reventa)
            {
                // Si los campos CUITCliente y Pedido no están vacíos
                if (!string.IsNullOrEmpty(orden.CUITCliente) && !string.IsNullOrEmpty(orden.Pedido))
                {
                    // Usa CUITDestinatario si no está vacío, de lo contrario usa CUITCliente
                    return !string.IsNullOrEmpty(orden.CUITDestinatario) ? orden.CUITDestinatario : orden.CUITCliente;
                }
                else
                {
                    // Usa CUITDestinatario si no está vacío, de lo contrario usa CUITDestino
                    return !string.IsNullOrEmpty(orden.CUITDestinatario) ? orden.CUITDestinatario : orden.CUITDestino;
                }
            }
            else
            {
                // En caso de Reventa
                // Usa CUITDestinatario si no está vacío, de lo contrario usa CUITDestino
                return !string.IsNullOrEmpty(orden.CUITDestinatario) ? orden.CUITDestinatario : orden.CUITDestino;
            }
        }

        private bool ValidarDummyActivo() 
        {
            var configuracionGeneral = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason, Constantes.ConfiguracionGeneral.CNRT.CNRTDummy, null);
            return configuracionGeneral is null ? false : bool.Parse(configuracionGeneral.Valor);
        }
        private Comando GenerarConsultaEscalables(string patente , string acoplado , string usuario)
        {
            if (ValidarDummyActivo())
            {
                return new ConsultarEscalablesDummy { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
            }

            else
            {
                return new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
            }

        }
        private string ObtenerDocumentoDesdeCuil(string cuil)
        {
            cuil = cuil.Replace("-", "");

            if (string.IsNullOrEmpty(cuil) || cuil.Length != 11)
            {
                throw new ArgumentException("El CUIL debe tener 11 dígitos.");
            }
            string documento = cuil.Substring(2, 8);

            return documento.TrimStart('0');
        }
    }

}