using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadIngresarOrdenCargaInterna)]
    public class IngresarOrdenCargaInternaController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<IIngresarOrdenCargaInternaService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioOperaciones servicioOperaciones;
        private readonly ICache cache;

        public IngresarOrdenCargaInternaController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<IIngresarOrdenCargaInternaService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows , IServicioOperaciones servicioOperaciones, ICache cache)
            : base(log, servicio, servicioComandos)
        {
            this.factory = factory;
            this.workflows = workflows;
            this.servicioOperaciones = servicioOperaciones;
            this.cache = cache;
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
            var numeroOrden = servicio.ObtenerNumeroDocumentoGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            var orden = new OrdenCargaInternaDto { FechaEmision = DateTime.Now, NumeroOrden = numeroOrden };
            if (cargaDeCupoId != 0)
            {
                var cupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
                if (cupo != null && cupo.MaterialId.HasValue)
                {
                    var resultadoConsultarOrdenInsumos = servicioComandos.Ejecutar(new ConsultarOrdenInsumos 
                    { 
                        Patente = cupo.Patente, 
                        MaterialId = cupo.MaterialId.Value, 
                        NumeroOrden = numeroOrden,
                        CentroId = cupo.CentroId,
                    }) as ResultadoConsultarOrdenInsumos;
                    if (resultadoConsultarOrdenInsumos.HayErrores && resultadoConsultarOrdenInsumos.Errores.ContainsKey("Error"))
                    {
                        ViewBag.ErrorAfip = resultadoConsultarOrdenInsumos.Errores["Error"];
                    }
                    else
                    {
                        ModelState.AgregarErrores(resultadoConsultarOrdenInsumos);
                        orden = resultadoConsultarOrdenInsumos.Dto;
                    }
                }
                else if (cupo != null)
                {
                    orden.PatenteCamion = cupo.Patente;
                }
            }
            return View(orden);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, OrdenCargaInternaDto orden, DatosUsuario datosUsuario)
        {
            var workflowObje = servicio.ObtenerWorkflowPorCodigo(workflow);
            ConsultarPagoTasaMunicipal(datosUsuario.CentroId, orden.PatenteCamion, orden.PatenteAcoplado, null, orden.TipoVehiculo, string.Empty, false, orden.MaterialId);

            Validar(orden);

            if (!ModelState.IsValid)
            {
                SetearVista(workflowObje, datosUsuario.CentroId);
                ViewBag.ErrorAfip = Textos.OrdenCarga_ErrorValidacion;
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
                TempData["Alerta"] = Textos.PatenteEnOtroWorkflow;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var resultadoChofer = SetearChofer(orden.Chofer);
            if (resultadoChofer == false)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }

            if(orden.DerivadoGranarioHabilitado && !(orden.Demorado || orden.Rechazado))
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
                    DestinoId = orden.DestinoId,
                    DestinoPlanta = orden.PlantaDGDestino ?? 0,
                    DestinoDomicilioTipo = orden.TipoDomicilioDestino ?? 0,
                    DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
                    TransportistaId = orden.TransportistaId,
                    Dominios = dominios.ToArray(),
                    KmRecorrer = !string.IsNullOrEmpty(orden.KmARecorrer) ? int.Parse(orden.KmARecorrer) : 0,
                    ChoferCuit = orden.Chofer.Cuil,
                    PagadorFleteId = orden.PagadorFleteId ?? 0,

                }) as ResultadoCartaPorteElectronicaDummy;
                if (resultadoAltaDummy.HayErrores)
                {
                    SetearVista(workflowObje, datosUsuario.CentroId);
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
                    SetearVista(workflowObje, datosUsuario.CentroId);
                    ViewBag.ErrorAfip = resultadoAnulacionDummy.Errores.Values.First();
                    return View(orden);
                }
            }

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaInterna,
                ActividadXaml = "IngresarOrdenCargaInterna",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Comentario = orden.Rechazado ? $"Vehiculo Rechazado. {orden.MotivoRechazo}" : (orden.Demorado ? $"Vehiculo Demorado. {orden.MotivoDemora}" : string.Empty)
            };

            int workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            
            var servicioWf = factory.CrearServicio(workflowDefinicionId);
            var resultadoActividad = servicioWf.IngresarOrdenCargaInterna(orden, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
            
            if (!resultadoActividad.HayErrores)
            {
                if (ResultadoPagoTasaMunicipal != null && ResultadoPagoTasaMunicipal.IdPago != 0)
                    ActualizarTasaMunicipal(resultadoActividad.InstanciaWorkflowId, ResultadoPagoTasaMunicipal.IdPago, ResultadoPagoTasaMunicipal.IdDiferenciaDePago ?? 0);

                return RedirectToAction("Index", "ListaDeCamiones", new { id = resultadoActividad.InstanciaWorkflowId });
            }

            ModelState.AgregarErrores(resultadoActividad);
            SetearVista(workflowObje, datosUsuario.CentroId);
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
            var almacenes = servicio.ListarAlmacenesPorCentro(centroId);
            controller.ViewBag.Almacenes = almacenes.GroupBy(x => x.Descripcion).Select(x => new AlmacenDto { Id = x.Select(y => y.Id).FirstOrDefault(), Descripcion = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Descripcion);
            var calles = servicio.ListarCalles(centroId);
            controller.ViewBag.Calles = calles.GroupBy(x => x.Nombre).Select(x => new CalleDto { Id = x.Select(y => y.Id).FirstOrDefault(), Nombre = x.Key }).ToSelectList(f => f.Id.ToString(), f => f.Nombre,"3");
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren ).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
            controller.ViewBag.TiposVehiculo.Insert(0, new SelectListItem { Text = Textos.Default_TipoVehiculo, Value = "" });
            controller.ViewBag.WorkflowId = workflow.Id;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.MaterialesDerivadoGranario = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();
        }

        [HttpGet]
        [DatosUsuario]
        public JsonResult ObtenerAlmacenesPorMaterial(int materialId, DatosUsuario datosUsuario)
        {
            var almacenes = servicio.ListarAlmacenesPorMaterial(datosUsuario.CentroId, materialId).OrderBy(c => c.Descripcion).Select(x => new AlmacenDto { Id = x.Id, Descripcion = x.Descripcion }).ToList();
            almacenes.Insert(0, new AlmacenDto { Descripcion = "(almacen)" });
            return Json(almacenes, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerOrdenDeCargaOperacionesPorPatente(string patente, DatosUsuario datosUsuario)
        {
            var response = new RespuestaEstandarDto<List<OrdenResiduosDto>>();

            try
            {
                var ordenes = this.OrdenesFiltradas(this.ObtenerRespuestaOrdenDeCargaOperaciones(patente));

                if (ordenes != null)
                {
                    var choferCuils = ordenes.Select(item => FormatterHelper.ConvertirCuilConGuionesSinException(item.CUILChofer)).ToList();
                    var choferes = servicio.ObtenerChoferesPorCuits(choferCuils.ToList());

                    foreach (var item in ordenes)
                    {
                        var choferCuil = FormatterHelper.ConvertirCuilConGuionesSinException(item.CUILChofer);
                        var chofer = choferes.FirstOrDefault(c => c.Cuil == choferCuil);

                        if (chofer == null)
                        {
                            var choferNuevo = new ChoferDto
                            {
                                Nombre = item.ChoferNombre,
                                Apellido = item.ChoferApellido,
                                TipoDocumentoIdentidadId = 1,
                                Cuil = choferCuil,
                                NumeroDeDocumento = FormatterHelper.ObtenerDocumentoDesdeCuilConGuiones(choferCuil)
                            };

                            var resultChofer = SetearChofer(choferNuevo);

                            if (!resultChofer)
                            {
                                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se pudo ingresar el chofer " + item.ChoferNombre + " del numero de orden: " + item.Id, TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                                break;
                            }
                        }
                    }

                    if (ordenes.Count == 0)
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se encontró ninguna Orden de Residuos con la patente ingresada", TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                    else
                        response.Data = ordenes;
                }
                else
                {
                    var cachedResponse = cache.Obtener<List<OrdenResiduosDto>>($"Operaciones:OrdenResiduosDto");
                    cachedResponse = cachedResponse.Where(q => q.PatenteChasis == patente).ToList();
                    if (cachedResponse.Count == 0)
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No se encontró ninguna Orden de Residuos con la patente ingresada", TipoDeMensaje = TipoDeMensajeDeRespuesta.Warning });
                    else
                        response.Data = cachedResponse;
                }
            }
            catch (Exception ex)
            {
                var mensaje = $"Ocurrio un error al consultar el servicio ObtenerOrdenesDeCarga con la patente {patente}";
                log.Error(ex, mensaje);
                throw;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerOrdenDeCargaOperacionesSeleccionada(string clienteCUIT, string transportistaCUIT, string patente, string acoplado, int materialSAP, string ordenId, string DestinoCUIT, DatosUsuario datosUsuario)
        {
            clienteCUIT = FormatterHelper.ConvertirCuilConGuionesSinException(clienteCUIT);
            transportistaCUIT = FormatterHelper.ConvertirCuilConGuionesSinException(transportistaCUIT);
            DestinoCUIT = FormatterHelper.ConvertirCuilConGuionesSinException(DestinoCUIT);

            var response = new RespuestaEstandarDto<OrdenDeCargaComplementariaDto>();

            try
            {
                var respuesta = servicio.ContarClientes(clienteCUIT);
                if (respuesta > 1) 
                    throw new InvalidOperationException("La secuencia contiene más de un elemento");

                var cliente = servicio.ObtenerClientePorCuit(clienteCUIT);
                var destino = servicio.ObtenerClientePorCuit(DestinoCUIT);
                var transportista = servicio.ObtenerProveedorPorCuit(transportistaCUIT, new TiposProveedor { PR = true });
                var material = servicio.ObtenerMaterialPorId(materialSAP);
                
                var resp = this.ObtenerRespuestaOrdenDeCargaOperaciones(patente);
                var orden = resp.FirstOrDefault(x => x.Id == Convert.ToInt32(ordenId));
                var choferCuil = FormatterHelper.ConvertirCuilConGuionesSinException(orden.CUILChofer);
                var chofer = servicio.ObtenerChoferPorCuit(choferCuil);

                var resultadoEscalables = servicioComandos.Ejecutar(GenerarConsultaEscalables(patente, acoplado, datosUsuario.NombreUsuario)) as ResultadoEscalables;

                if (cliente == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No se encontró un Cliente para el cuit {clienteCUIT}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (transportista == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No se encontró un Transportista para el cuit {transportistaCUIT}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (material == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No existe material con el codigo de SAP {materialSAP}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

                if (resultadoEscalables.HayErrores)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"Error al obtener el tipo de vehículo por patente: {resultadoEscalables.Errores.Values.First()}", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }

                if (response.EsValido)
                {
                    orden.FechaCreacion = DateTime.Now.ToString();
                    var ordenDeCargaComplementario = new OrdenDeCargaComplementariaDto
                    {
                        ClienteId = cliente?.Id,
                        ClienteDescripcion = cliente?.Descripcion ?? "",
                        TransportistaId = transportista?.Id,
                        TransportistaDescripcion = transportista?.RazonSocial ?? "",
                        TipoDeVehiculo = (int)(resultadoEscalables.Categoria ?? TipoVehiculo.Camión),
                        MaterialId = material.Id,
                        EsDerivadoGranario = material.EsDerivadoGranario,
                        Orden = orden,
                        TieneErrorCNRT = resultadoEscalables.HayErrores,
                        DestinoId = destino?.Id,
                        DestinoDescripcion = destino?.Descripcion ?? "",
                    };

                    response.Data = ordenDeCargaComplementario;
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException ex)
            {
                return HandleException(ex, patente, true);
            }
            catch (Exception ex)
            {
                return HandleException(ex, patente, false);
            }
        }

        private JsonResult HandleException(Exception ex, string patente, bool duplicado)
        {
            var mensaje = $"Ocurrio un error al consultar el servicio ObtenerOrdenesDeCarga con la patente {patente}";
            log.Error(ex, mensaje);

            var data = new
            {
                success = false,
                errorResponse = new
                {
                    error = ex.Message,
                    duplicado
                }
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<OrdenResiduosDto> OrdenesFiltradas(List<OrdenResiduosDto> ordenDeCargaDtos)
        {
            return ordenDeCargaDtos.Where(orden => !servicio.ExisteOrdenCarga(orden.Id.ToString())).ToList();
        }

        private void Validar(OrdenCargaInternaDto orden)
        {
            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;

            if (material != null && material.Descripcion == "RESIDUOS ORGANICOS" && orden.Almacen_Id == null)
                ModelState.AddModelError(nameof(OrdenCargaInternaDto.Almacen_Id), Textos.OrdenInterna_AlmacenRequerido);

            if (material.EsDerivadoGranario && (string.IsNullOrEmpty(orden.KmARecorrer) || orden.KmARecorrer.Length > 4 || !int.TryParse(orden.KmARecorrer, out int km) || km <= 0))
                ModelState.AddModelError(nameof(OrdenCargaInternaDto.KmARecorrer), string.Format(Textos.Error_Requerido, Textos.CartaPorte_KmRecorrer));
            
            if (material.EsDerivadoGranario && !orden.PlantaDGDestino.HasValue)
                ModelState.AddModelError(nameof(OrdenCargaInternaDto.PlantaDGDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
            
            if (material.EsDerivadoGranario && string.IsNullOrEmpty(orden.TipoYOrdenDestino))
                ModelState.AddModelError(nameof(OrdenCargaInternaDto.TipoYOrdenDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));

            if (material.EsDerivadoGranario && (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0)) 
                ModelState.AddModelError(nameof(OrdenCargaInternaDto.PagadorFlete), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
        }

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
        }

        private List<OrdenResiduosDto> ObtenerRespuestaOrdenDeCargaOperaciones(string patente)
        {
            IEnumerable<OrdenResiduosDto> data = servicioOperaciones.ObtenerOrdenesResiduos(patente);
            return data.ToList();
        }

        private bool ValidarDummyActivo()
        {
            var configuracionGeneral = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason, Constantes.ConfiguracionGeneral.CNRT.CNRTDummy, null);
            return configuracionGeneral is null ? false : bool.Parse(configuracionGeneral.Valor);
        }

        private Comando GenerarConsultaEscalables(string patente, string acoplado, string usuario)
        {
            if (ValidarDummyActivo())
                return new ConsultarEscalablesDummy { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
            else
                return new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
        }
    }
}