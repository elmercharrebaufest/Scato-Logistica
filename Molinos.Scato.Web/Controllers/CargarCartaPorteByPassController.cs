using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using NPOI.Util;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCargarCartaPorteByPass)]
    public class CargarCartaPorteByPassController : DocumentoIngresoController
    {
        #region -- Fields --

        private readonly IServicioActividadFactory<ICargarCartaPorteByPassService> factory;
        private readonly IListaDeWorkflows workflows;
        protected readonly IFirmaProvider configuracion;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioOrquestador servicioOrquestador;

        #endregion

        #region -- Constructors --

        public CargarCartaPorteByPassController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteByPassService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador)
            : base(log, servicio, servicioComandos)
        {
            this.configuracion = configuracion;
            this.factory = factory;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
            this.servicioOrquestador = servicioOrquestador;
        }

        #endregion

        #region -- Methods --

        [DatosUsuario]
        public virtual ActionResult Index(string workflow, DatosUsuario datosUsuario, string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "", int cargaDeCupoId = 0)
        {
            ViewBag.FotoMesaDigitalizacionSustentable = null;
            ViewBag.Usuario = datosUsuario.NombreUsuario;
            ViewBag.AceptaRechazar = true;
            log.Debug("Cookie Usuario: {0}", new CookieUsuario());
            if (!servicio.WorkflowActivoConDefinicionActiva(workflow))
            {
                TempData["Alerta"] = Textos.Error_WorkflowSinDefinicionActiva;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }

            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            SetearVista(workflowObj, datosUsuario.CentroId);

            if (String.IsNullOrEmpty(destinatarioCodigoSap) && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
            {
                destinatarioCodigoSap = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            }

            var carta = servicio.ObtenerCartaPorteVacia(datosUsuario.CentroId, workflow, destinatarioCodigoSap, titularCodigoSap, centroDestino, rtteComercial);
            //var cargaCupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
            carta.EsClienteDestinatario = false;
            if (cargaDeCupoId > 0)
            {
                var carga = servicio.ObtenerCupoPorId(cargaDeCupoId);
                carta.NroCartaPorte = carga.CPE ? carga.CTG : carga.NumeroCartaPorte;
                carta.Cpe = carga.CPE;
                ViewBag.NroCartaPorteGarita = carga.NumeroCartaPorte;
                if (!string.IsNullOrEmpty(carga.FotoRutaDestino))
                {
                    var foto = servicio.ObtenerFotoPorPath(carga.FotoRutaDestino);
                    if (foto.Fotos.Any())
                    {
                        var path = Path.GetDirectoryName(carga.FotoRutaDestino).Replace("temp", "");
                        ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
                        ViewBag.PuestoDeTrabajo = path;
                    }
                }
                if (!string.IsNullOrEmpty(carga.FotoRutaSustentable))
                {
                    var foto = servicio.ObtenerFotoPorPath(carga.FotoRutaSustentable);
                    if (foto.Fotos.Any())
                    {
                        ViewBag.FotoMesaDigitalizacionSustentable = foto.Fotos.First().Foto;
                    }
                }
                if (!(carga is null))
                {
                    var vehiculos = new List<VehiculoDto> { new VehiculoDto { Patente = carga.Patente } };
                    carta.Patente = carga.Patente;
                    carta.MaterialId = carga.MaterialId.GetValueOrDefault();
                    carta.Material = carga.MaterialDescripcion;
                    carta.VehiculoJson = vehiculos.ToJson();
                    carta.Cupo = carga.Cupo;
                }
            }
            return View(carta);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public virtual ActionResult Index(string workflow, string puestoDeTrabajo, string fotoMesaDigitalizacion1, string fotoMesaDigitalizacion2, CartaPorteDto orden, DatosUsuario datosUsuario)
        {
            orden.FechaVto = DateTime.Now;
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;

            if (string.IsNullOrEmpty(orden.NroCartaPorte) && orden.TipoVehiculoInt == (int)TipoVehiculo.Tren)
            {
                var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                orden.NroCartaPorte = $"{DateTime.Now:yyyyMMdd}{sequenciaNroCartaPorteCPE:D4}";
                ModelState.Remove("NroCartaPorte");
            }

            if (orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Egreso)
            {
                if (string.IsNullOrEmpty(orden.NroCartaPorte))
                {
                    var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                    orden.NroCartaPorte = $"{DateTime.Now:yyyyMMdd}{sequenciaNroCartaPorteCPE:D4}";
                }

                ModelState.Remove("NroCartaPorte");
                ModelState.Remove("CTG");
            }

            if (orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && servicio.ValidarCPERedespacho(orden.NumeroOperativo?.ToString() ?? orden.NroCartaPorte, datosUsuario.CentroId, workflow, orden.TipoVehiculoInt))
            {
                ModelState.Remove("NroCartaPorte");
                ModelState.Remove("CTG");
            }

            if (datosUsuario.CentroId == 0)
            {
                log.Debug("El usuario {0} no tiene seleccionado un centro", datosUsuario.NombreUsuario);
                TempData["Alerta"] = Textos.SeleccionarCentro_Error;
                TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            if (!Validar(orden, datosUsuario))
            {
                SetearVista(workflowObj, datosUsuario.CentroId);
                return View(orden);
            }
            if (ModelState.IsValid && vehiculos != null && vehiculos.Any())
            {
                if (servicio.TomaFotoEnMesa(datosUsuario.CentroId))
                {
                    if (string.IsNullOrEmpty(fotoMesaDigitalizacion1))
                    {
                        log.Debug("Foto de CP 1 es requerida: {1}", orden.NroCartaPorte);
                        ModelState.AddModelError("", Textos.Error_FotoCP);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                    if (vehiculos.First().TipoVehiculo == TipoVehiculo.Tren && !orden.Cpe && string.IsNullOrEmpty(fotoMesaDigitalizacion2))
                    {
                        log.Debug("Foto de CP 2 es requerida: {1}", orden.NroCartaPorte);
                        ModelState.AddModelError("", Textos.Error_FotoCP);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                }

                var response = ValidarNumeroCartaPorte(orden, datosUsuario.CentroId, workflow);
                if (!response.Valida)
                {
                    log.Debug("No se puede crear la CP {0}. Detalle: {1}", orden.NroCartaPorte, response.Error);
                    ModelState.AddModelError("NroCartaPorte", response.Error);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (vehiculos.Any(vehiculo => !PatenteValida(vehiculo.Patente, orden.TipoVehiculo)
                    || (!string.IsNullOrEmpty(vehiculo.PatenteAcoplado) && !PatenteValida(vehiculo.PatenteAcoplado, orden.TipoVehiculo))
                    || (!string.IsNullOrEmpty(vehiculo.PatenteAcoplado2) && !PatenteValida(vehiculo.PatenteAcoplado2, orden.TipoVehiculo))))
                {
                    log.Debug("No se puede crear la CP {0}. Alguna de las patentes no respeta el formato ABC123 o AB123CD");
                    ModelState.AddModelError("", Textos.CargaDeCupo_Patente_ErrorFormato);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (vehiculos.GroupBy(x => x.Patente).Any(g => g.Count() > 1))
                {
                    log.Debug("No se puede crear la CP {0}. Alguna de las patentes está duplicada");
                    ModelState.AddModelError("", Textos.CartaPorte_PatenteDuplicadaError);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (vehiculos.Any(vehiculo => workflows.ObtenerWorkflowPorPatente(vehiculo.Patente) != null))
                {
                    log.Debug("No se puede crear la CP. Alguna de las patentes esta ingresada en un workflow en ejecución");
                    ModelState.AddModelError("", Textos.OrdenCargaInterna_PatenteEnOtroWorkflow);
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var tipoComercial = servicio.ObtenerTipoComercial(orden.TipoComercialId);

                if (tipoComercial?.PesoMaximoDocumentoIngreso != null && tipoComercial.PesoMaximoDocumentoIngreso != 0)
                {
                    if (vehiculos.Any(vehiculo => vehiculo.PesoBrutoOrigen > tipoComercial.PesoMaximoDocumentoIngreso))
                    {
                        log.Debug("El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                        ModelState.AddModelError("", Textos.OrdenCargaInterna_PesoMaximoExcedido);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                }

                if (!SetearChofer(orden.Chofer))
                {
                    log.Debug("No se pudo dar de alta o asociar el chofer a la CP");
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId ?? 0;
                if (!SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista))
                {
                    log.Debug("No se pudo dar de alta o asociar el transportista a la CP");
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }
                orden.TransportistaId = transportistaId;

                if (orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && orden.Cpe && orden.TransportistaTramo2Id.HasValue && orden.TransportistaTramo2Id != 0)
                {
                    var transportistaTramo2Id = orden.TransportistaTramo2Id.Value;
                    if (!SetearTransportista(ref transportistaTramo2Id, orden.TipoComercialId, orden.EsTransportistaTramo2, true))
                    {
                        log.Debug("No se pudo dar de alta o asociar el transportista 2 a la CP");
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                    orden.TransportistaTramo2Id = transportistaTramo2Id;
                }

                if (!this.ValidarCuposDeIngresoyEgreso(orden, datosUsuario, workflowObj))
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }
               
                var tieneException = ExisteExcepcionAlControlParaCartaPorte(servicio, orden.MaterialId, orden.TransportistaId ?? 0, orden.IntermediarioFleteId, datosUsuario.CentroId, DateTime.Today, orden.DestinoId, null);
                var transportista = servicio.ObtenerTransportista(orden.TransportistaId ?? 0);
                var proveedor = servicio.ObtenerProveedor(orden.IntermediarioFleteId);

                if (!tieneException)
                {
                    var comandoVerificarCompliance = new VerificarEnCompliance
                    {
                        Cuit = orden.IntermediarioFlete != null ? proveedor.Cuil  : transportista.Cuit,
                        Dni = orden.Chofer.NumeroDeDocumento,
                        Patente = vehiculos.First().Patente,
                        Planta = datosUsuario.CentroId.ToString()
                    };
                    log.Debug("Se valida compliance Cuit: {0}, Dni: {1}, Patente: {2}, Planta: {3} ",
                        comandoVerificarCompliance.Cuit,
                        comandoVerificarCompliance.Dni,
                        comandoVerificarCompliance.Patente,
                        comandoVerificarCompliance.Planta);

                    var resultado = servicioComandos.Ejecutar(comandoVerificarCompliance) as ResultadoValidarCompliance;

                    if (resultado.HayErrores)
                    {
                        ModelState.AddModelError("", resultado.Errores.Values.First());
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }

                }

                orden.Vehiculos = vehiculos;
                orden.FechaEmision = DateTime.Now;
                orden.TipoDeWorkflow = workflowObj.TipoDeWorkflow;
                orden.EsClienteDestinatario = false;
                orden.CodEstab = string.IsNullOrEmpty(orden.CodEstab) ? "999999" : orden.CodEstab;
                var i = 1;
                foreach (var vehiculo in vehiculos)
                {
                    vehiculo.TipoVehiculo = orden.TipoVehiculo;
                    vehiculo.Patente = vehiculo.Patente?.ToUpper() ?? "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado?.ToUpper() ?? "";
                    vehiculo.PatenteAcoplado2 = vehiculo.PatenteAcoplado2?.ToUpper() ?? "";
                    vehiculo.NumeroVehiculo = i++;
                }
                var fecha = DateTime.Now;
                orden.FotoRutaDestino = GuardarfotoMesaDigitalizacion(fotoMesaDigitalizacion1, orden, puestoDeTrabajo, datosUsuario, fecha);
                var cupo = servicio.ObtenerCupoPorCupoSap(orden.Cupo);
                if (cupo?.Especial == true)
                {
                    orden.FotoRutaSustentable = GuardarfotoMesaDigitalizacionSelloSustentable(orden, datosUsuario, fecha);
                }
                if (!string.IsNullOrEmpty(fotoMesaDigitalizacion2))
                {
                    orden.FotoRutaDestinoDetalle = GuardarfotoMesaDigitalizacion(fotoMesaDigitalizacion2, orden, puestoDeTrabajo, datosUsuario, fecha.AddMinutes(1));
                }

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var instanceIds = new List<Guid>();

                log.Info("CargarCartaPorteByPass: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                foreach (var vehiculo in vehiculos)
                {
                    if (orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
                    {
                        var ctgVagon = Convert.ToInt64(string.IsNullOrEmpty(vehiculo?.NumCTG) ? orden.NroCartaPorte : vehiculo?.NumCTG);
                        var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = ctgVagon }) as ResultadoConsultarImagenCpe;
                        if (!cartaPorteImagen.HayErrores)
                        {
                            var imagenBase64 = Convert.ToBase64String(cartaPorteImagen.PdfImage);
                            orden.FotoRutaDestino = GuardarfotoMesaDigitalizacion(imagenBase64, orden, puestoDeTrabajo, datosUsuario, fecha, ctgVagon.ToString());
                            if (cupo?.Especial == true)
                            {
                                orden.FotoRutaSustentable = GuardarfotoMesaDigitalizacionSelloSustentable(orden, datosUsuario, fecha);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(vehiculo.Patente) && orden.TransportistaId.HasValue)
                    {
                        var huella = servicio.ObtenerHuellaDigitalPorFiltro(vehiculo.Patente, vehiculo.PatenteAcoplado, orden.TransportistaId.Value);

                        if (huella is null)
                        {
                            TempData["Alerta"] = "El vehículo debe realizar pesaje tara en balanza de Acopio";
                            TempData["TipoAlerta"] = TipoAlerta.Advertencia;
                            return RedirectToAction("Index", "ListaDeCamiones");
                        }
                    }

                    var controlRecorrido = GenerarControlRecorrido(datosUsuario);
                    var resultadoActividad = servicioWf.CargarCartaPorteByPass(orden, vehiculo, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                    if (resultadoActividad.HayErrores)
                    {
                        ModelState.AgregarErrores(resultadoActividad);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                    orden.Id = resultadoActividad.Id;
                    instanceIds.Add(resultadoActividad.InstanciaWorkflowId);
                }

                ViewBag.Headers = new Dictionary<string, string> { { "WFInstanceIds", string.Join(",", instanceIds) } };

                if (orden.VehiculoDemorado)
                {
                    servicioComandos.Ejecutar(new EnviarMensajeCamioneroCircular
                    {
                        CartaPorte = orden.Cpe ? orden.CTG : orden.NroCartaPorte,
                        Mensaje = $"El camión ha sido demorado por el siguiente motivo: {orden.MotivoDemora}",
                        SePuedeDesactivar = false
                    });
                }

                return RedirectToAction("Index", "ListaDeCamiones", new { id = instanceIds[0] });
            }
            if (vehiculos == null || !vehiculos.Any())
            {
                ModelState.AddModelError("Vehiculo", string.Format(Textos.Error_Requerido, Textos.Vagones));
            }

            SetearVista(workflowObj, datosUsuario.CentroId);
            return View(orden);
        }

        public ActionResult MostrarCamion(CartaPorteDto model)
        {
            return View("Camion", model);
        }

        public ActionResult MostrarVagones(CartaPorteDto model)
        {
            return View("Vagon", model);
        }

        public ActionResult MostrarCamionJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorteByPass/Camion.cshtml");
            }

            var modelo = model.FromJson<CartaPorteDto>();
            return View("~/Views/CargarCartaPorteByPass/Camion.cshtml", modelo);
        }

        public ActionResult MostrarVagonesJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorteByPass/Vagon.cshtml");
            }
            var modelo = model.FromJson<CartaPorteDto>();
            return View("~/Views/CargarCartaPorteByPass/Vagon.cshtml", modelo);
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorte(string numero, string workflow, DatosUsuario datosUsuario)
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

        [DatosUsuario]
        public JsonResult ObtenerCartaPorteCtg(string numeroCtg, string workflow, DatosUsuario datosUsuario)
        {
            try
            {
                log.Debug("Obteniendo CTG {0} workflow {1}", numeroCtg, workflow);
                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarDetalleCTG { CentroId = datosUsuario.CentroId, Ctg = numeroCtg, Usuario = datosUsuario.NombreUsuario }) as ResultadoDetalleCTG;

                log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener carta de porte CTG{0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo carta de porte CTG {0}", numeroCtg);

                return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "0", Error = cartaPorteResponse.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Info(e, "No se pudo obtener la carta de porte CTG {0}", numeroCtg);
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorteCtgCompraGranos(long numeroCtg, string workflow, int tipoVehiculo, bool consultactg, DatosUsuario datosUsuario)
        {
            try
            {
                if (servicio.ValidarCPERedespacho(numeroCtg.ToString(), datosUsuario.CentroId, workflow, tipoVehiculo, consultactg))
                {
                    return Json(new { CodigoDeError = "2", Error = "No existen solicitudes para los parámetros indicados." }, JsonRequestBehavior.AllowGet);
                }

                log.Debug("Obteniendo CTG {0} workflow {1}", numeroCtg, workflow);

                if (tipoVehiculo != (int)TipoVehiculo.Tren)
                {
                    var cartaPorteResponseDB = servicio.ObtenerCartaPorteAReutilizarPorNumero(numeroCtg.ToString(), datosUsuario.CentroId, workflow);
                    if (cartaPorteResponseDB.CodigoDeError != 1)
                    {
                        var vehiculo = cartaPorteResponseDB.CartaPorte.Vehiculos.FirstOrDefault();
                        var categoriaVehiculo = servicio.BuscarCategoriaVehiculo(vehiculo.Patente, vehiculo.PatenteAcoplado, vehiculo.PatenteAcoplado2);
                        if (categoriaVehiculo != null)
                        {
                            cartaPorteResponseDB.CartaPorte.TipoVehiculo = (Dominio.Enums.TipoVehiculo)categoriaVehiculo.TipoVehiculo;
                        }
                        return Json(new { Cpe = cartaPorteResponseDB.CartaPorte, cartaPorteResponseDB.CodigoDeError, cartaPorteResponseDB.Error }, JsonRequestBehavior.AllowGet);
                    }
                }

                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCPDigital
                {
                    NroCtg = numeroCtg,
                    Usuario = datosUsuario.NombreUsuario,
                    CentroId = datosUsuario.CentroId,
                    TipoVehiculo = tipoVehiculo,
                    ConsultaFerroviarioPorCtg = consultactg
                }) as ResultadoCartaPorteElectronica;

                var errorCode = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "3";
                var errorMsg = cartaPorteResponse.Errores.Values.FirstOrDefault();
                var estadoCPE = cartaPorteResponse.Cpe?.EstadoCpe?.ToUpper()?.Trim();

                if (!cartaPorteResponse.HayErrores && !EstadosCPEdeAFIP.Validos.Contains(estadoCPE))
                {
                    if (EstadosCPEdeAFIP.Bloqueantes.Contains(cartaPorteResponse.Cpe?.EstadoCpe))
                    {
                        errorMsg = $"El CTG {numeroCtg} se encuentra en estado {EstadosCPEdeAFIP.Descripciones[estadoCPE]}";
                        errorCode = "5";
                    }
                    else
                    {
                        errorMsg = $"El CTG {numeroCtg} no se encuentra en estado ACTIVO";
                        errorCode = "4";
                    }
                }

                if (cartaPorteResponse.Cpe.Cupo != null && cartaPorteResponse.Cpe.Cupo.Equals("MOL1111/11111111"))
                {
                    ModelState.AddModelError(nameof(cartaPorteResponse.Cpe.Cupo), "El cupo no puede ser generico");
                }

                log.Debug(cartaPorteResponse.HayErrores ? $"Error al obtener carta de porte CTG-CPE {numeroCtg}: {cartaPorteResponse.Errores.Values.First()}" : $"Devolviendo carta de porte CTG-CPE {numeroCtg}");
                return Json(new { cartaPorteResponse.Cpe, CodigoDeError = errorCode, Error = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Info(e, $"No se pudo obtener la carta de porte CTG-CPE {numeroCtg}");
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult TomarFotoMesaDigitalizacion(DatosUsuario datosUsuario)
        {
            var puestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).ToArray();
            if (puestosDeTrabajo.SelectMany(x => x.VideoCamaras).Any() && servicio.TomaFotoEnMesa(datosUsuario.CentroId))
            {
                var videocamara = puestosDeTrabajo.SelectMany(x => x.VideoCamaras).First();
                try
                {
                    //var i = Image.FromFile("C:\\Users\\aolivera\\Downloads\\Fotos\\b.jpeg");
                    //var resultado = Json(new { CodigoDeError = 0, PuestoDeTrabajo = puestosDeTrabajo.First().NombrePuesto, Foto = Convert.ToBase64String(i.ToByteArray()) }, JsonRequestBehavior.AllowGet);
                    //resultado.MaxJsonLength = int.MaxValue;
                    //return resultado;

                    log.Debug("Ejecutando Camara : {0}", videocamara.Codigo);
                    var resultado = servicioOrquestador.Ejecutar(
                        new EjecutarTomarFoto
                        {
                            CodigoDispositivo = videocamara.Codigo
                        });
                    if (resultado.Mensaje.Codigo != 0)
                    {
                        log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                        return Json(new { CodigoDeError = 1, Error = resultado.Mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CodigoDeError = 0, PuestoDeTrabajo = videocamara.Directorio, Foto = Convert.ToBase64String(((ResultadoTomarFoto)resultado).Imagen) }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "Fallo la foto del dispositivo: {0}", videocamara.Codigo);
                    return Json(new { CodigoDeError = 2, Error = "Error al obtener la imagen: " + e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { CodigoDeError = 3, Error = "No hay camaras configuradas para el puesto " + datosUsuario.NombrePc }, JsonRequestBehavior.AllowGet);
        }

        private string GuardarfotoMesaDigitalizacion(string fotoMesaDigitalizacion, CartaPorteDto orden, string directorio, DatosUsuario datosUsuario, DateTime fecha, string numCtg = null)
        {
            if (!string.IsNullOrEmpty(fotoMesaDigitalizacion))
            {
                log.Debug($"GuardarfotoMesaDigitalizacion  {fotoMesaDigitalizacion.Count()} {directorio} {fecha}");
                var path = servicioComandos.Ejecutar(
                    new GuardarfotoMesaDigitalizacion
                    {
                        Fecha = fecha,
                        FotoMesaDigitalizacion = fotoMesaDigitalizacion,
                        Directorio = directorio,
                        CentroId = datosUsuario.CentroId,
                        NumeroDocumentoIngreso = !string.IsNullOrEmpty(numCtg) ? numCtg : orden.NroCartaPorte,
                        TipoVehiculo = orden.Vehiculos.First().TipoVehiculo,
                        Usuario = datosUsuario.NombreUsuario,
                        Patente = orden.Vehiculos.First().Patente
                    }) as ResultadoGuardarFoto;

                return path != null ? path.Path : null;
            }
            return null;
        }

        protected virtual void SetearVista(WorkflowDto workflow, int centroId)
        {
            SetearVista(workflow, centroId, servicio, this);
        }

        public static void SetearVista(WorkflowDto workflow, int centroId, IServicioRepositorio servicio, ControllerBase controller)
        {
            var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
            var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
            var centro = servicio.ObtenerCentro(centroId);
            var ramalFerroviario = servicio.ListarRamalFerroviario();
            var pesoMaximoPorTipoVehiculo = servicio.ListarPesoMaximoPorTipoVehiculoPorCentro(centroId);
            controller.ViewBag.ControlarTiempoPorCTG = false;
            controller.ViewBag.RequiereNumeroAduana = false;
            controller.ViewBag.TiposComerciales = tiposComerciales.ToSelectList(f => f.Id.Value.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            controller.ViewBag.TiposComercialesTransportista = tiposComerciales.Where(x => !x.TransportistaEsProveedor).Select(y => y.Id.ToString()).ToList();
            controller.ViewBag.MaterialesConAnexo = materiales.Where(x => x.RequiereAnexoInase).Select(y => y.MaterialId.ToString()).ToList();
            controller.ViewBag.Materiales = materiales.ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            controller.ViewBag.TiposDocumentos = servicio.ListarTiposDocumentoIdentidad().ToSelectList(f => f.Id.ToString(), f => f.DescripcionCorta);
            controller.ViewBag.BocasDestino = new List<SelectListItem>();
            controller.ViewBag.Workflow = workflow.Codigo;
            controller.ViewBag.WorkflowDescripcion = workflow.Descripcion;
            controller.ViewBag.EsIngreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            controller.ViewBag.EsEgreso = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso;

            controller.ViewBag.RequiereCupo = centro.RequiereCupo;
            controller.ViewBag.ValidarCupo = centro.ValidarCupo;
            controller.ViewBag.DescargaCartaPortePorCtg = centro.DescargaCartaPortePorCtg && workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso;
            controller.ViewBag.TiposCategorias = servicio.ListarCategorias().ToSelectList(f => f.Id.ToString(),
                                                       f => f.Clasificacion);
            controller.ViewBag.CentroId = centroId;
            controller.ViewBag.ListaMateriales = materiales;
            var materialesConTecnologia = materiales.Where(x => x.RequiereTecnologia).Select(y => y.MaterialId.ToString()).ToList();
            if (materialesConTecnologia.Any())
            {
                controller.ViewBag.MaterialesConTecnologia = materialesConTecnologia;
                controller.ViewBag.Tecnologias = servicio.ListarTecnologias()
                                                         .ToSelectList(f => f.Id.ToString(),
                                                                       f => "(" + f.Codigo + ") " + f.Nombre);
            }
            else
            {
                controller.ViewBag.MaterialesConTecnologia = new List<string>();
                controller.ViewBag.Tecnologias = new List<SelectListItem>();
            }
            controller.ViewBag.ListaRamalFerroviario = ramalFerroviario.ToSelectList(f => f.CodigoAfip.ToString(), f => f.Descripcion);
            controller.ViewBag.AceptaPendiente = true;
            controller.ViewBag.TiposVehiculo = pesoMaximoPorTipoVehiculo.Where(x => x.TipoVehiculo != TipoVehiculo.Tren).ToSelectList(f => ((int)f.TipoVehiculo).ToString(), f => f.TipoVehiculo.DisplayText());
        }

        protected virtual CartaPorteValidaResponseDto ValidarNumeroCartaPorte(CartaPorteDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroCartaPorteValido(orden.NroCartaPorte, centroId, workflowCodigo, orden.Cpe);
        }

        protected virtual bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            var firmaSinLogo = configuracion.ObtenerFirmaSinLogo();
            var codigoSapMolinosAgro = firmaSinLogo.CodigoSAP;
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId).CodigoSap;
            var remitente = servicio.ObtenerProveedor(orden.RtteComercialId);
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);

            var codigoEstablecimientoEsDeMolinos = servicio.ObtenerCodigoEstablecimientoEsDeMolinos(orden.CodEstab);

            //si es MRP, no se valida el codigo de establecimiento
            if (codigoSapTitular == codigoSapMRP && (remitente == null || remitente.CodigoSap == codigoSapMRP || remitente.CodigoSap == codigoSapMolinosAgro))
            {
                ModelState.AddModelError("", Textos.Error_CCPPCompra);
                return false;
            }
            else if (codigoSapTitular == codigoSapMolinosAgro && (remitente == null || remitente.CodigoSap == codigoSapMolinosAgro) && codigoEstablecimientoEsDeMolinos)
            {
                ModelState.AddModelError("", Textos.Error_CCPPCompra);
                return false;
            }
            if (otroRecorridoDelChofer != null && orden.TipoVehiculo != TipoVehiculo.Tren)
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }
            return true;
        }

        #region -- Validar Cupos de Ingreso y Egreso --

        private bool ValidarCuposDeIngresoyEgreso(CartaPorteDto cartaPorteDto, DatosUsuario datosUsuario, WorkflowDto workflowObj)
        {
            bool cuposValidos = false;

            if (!cartaPorteDto.RequiereCupo || // Valida si no requiere cupo contra el centro
                !cartaPorteDto.ValidarCupo || // Valida si no requiere validar cupo contra el centro
                                              //TODO: Ver si hace falta hacer el check de workflow de ingreso o egreso y las validaciones de si es cliente destinatario
                (!workflowObj.TipoDeWorkflow.Equals(TipoDeWorkflow.Ingreso) && !cartaPorteDto.Cupo.StartsWith("MOL")) || // Valida si no es un cupo de MOA
                (!workflowObj.TipoDeWorkflow.Equals(TipoDeWorkflow.Ingreso) && cartaPorteDto.EsClienteDestinatario)) // Se toma como válido si es un cliente destinatario (destino no es MOA)
            {
                log.Info(
                    "No se requiere validar cupo de ingreso o egreso. Detalle: " + 
                    $"RequiereCupo -> {cartaPorteDto.RequiereCupo} | ValidarCupo -> {cartaPorteDto.ValidarCupo} | " +
                    $"TipoDeWorkflow -> {workflowObj.TipoDeWorkflow} -> Cupo no comienza con MOL -> { cartaPorteDto.Cupo } -> EsClienteDestinatario");

                cuposValidos = true;
            }
            else
            {
                cuposValidos = 
                    this.ValidarCupoDeIngreso(cartaPorteDto, datosUsuario) && 
                    this.ValidarCupoDeEgreso(cartaPorteDto, datosUsuario);
            }

            return cuposValidos;
        }

        #region -- ValidarCupoDeIngreso --

        private bool ValidarCupoDeIngreso(CartaPorteDto cartaPorteDto, DatosUsuario datosUsuario)
        {
            bool esCupoDeIngresoValido = true;

            var resultado = servicio.ValidarCupo(cartaPorteDto.Cupo, datosUsuario.CentroId, cartaPorteDto.NroCartaPorte);

            //TODO: Verificar si es necesario validar el cupo en SAP antes de verificar si ya fue asignado, comparado con CargarCartaPorteController.
            var validarCupoEnSAPResponse = this.ValidarCupoEnSAP(cartaPorteDto.Cupo, datosUsuario.CentroId , nameof(cartaPorteDto.Cupo));

            if (validarCupoEnSAPResponse.EsValido &&
                validarCupoEnSAPResponse.MaterialId != 0)
            {
                var resultadoCargaDeCupo =
                    this.CrearCargaDeCupo(
                        datosUsuario.CentroId,
                        cartaPorteDto.Cupo,
                        validarCupoEnSAPResponse.MaterialId,
                        datosUsuario.PuestoDeTrabajoId,
                        validarCupoEnSAPResponse.EsEspecial,
                        validarCupoEnSAPResponse.CupoSAP);

                if (resultadoCargaDeCupo.HayErrores)
                {
                    ModelState.AgregarErrores(resultadoCargaDeCupo);
                    esCupoDeIngresoValido = false;
                }
            }

            if (resultado.Reingresado != null)
            {
                var resultadoCargaDeCupo = servicioComandos.Ejecutar(new CrearCargaDeCupo { Dto = resultado.Reingresado });
                
                if (resultadoCargaDeCupo.HayErrores)
                {
                    ModelState.AddModelError("Cupo", resultadoCargaDeCupo.Errores.First().Value);
                    esCupoDeIngresoValido = false;
                }
            }

            if (esCupoDeIngresoValido && resultado.YaAsignado)
            {
                ModelState.AddModelError("Cupo", resultado.MensajeError);
                esCupoDeIngresoValido = false;
            }

            if (!cartaPorteDto.Cupo.Equals(Constantes.ValoresPorDefecto.CupoGenerico))
            {
                esCupoDeIngresoValido &= validarCupoEnSAPResponse.EsValido;
            }
            

            return esCupoDeIngresoValido;
        }

        private Resultado CrearCargaDeCupo(int centroId, string cupo, int materialId, int puestoDeTrabajoId, bool esEspecial, ZMPES5220 respuestaSAP)
        {
            if (respuestaSAP == null)
                throw new ArgumentNullException(nameof(respuestaSAP));

            var resultadoCargaDeCupo = servicioComandos.Ejecutar(
                new CrearCargaDeCupo
                {
                    Dto = new CargaDeCupoDto
                    {
                        CentroId = centroId,
                        Cupo = cupo,
                        Fecha = DateTime.Now,
                        FechaSap = respuestaSAP.FECHA,
                        MaterialId = materialId,
                        PuestoDeTrabajoId = puestoDeTrabajoId,
                        RespuestaSap = respuestaSAP.MENSAJE,
                        Camara = respuestaSAP.CALIDAD,
                        Especial = esEspecial
                    }
                });

            return resultadoCargaDeCupo;
        }

        #endregion

        #region -- ValidarCupoDeEgreso --

        private bool ValidarCupoDeEgreso(CartaPorteDto cartaPorteDto, DatosUsuario datosUsuario)
        {
            bool esCupoDeEgresoValido = true;

            if (cartaPorteDto.CupoSalida.Equals(Constantes.ValoresPorDefecto.CupoGenerico))
            {
                ModelState.AddModelError(nameof(cartaPorteDto.CupoSalida), "El cupo de salida no puede ser genérico");
                esCupoDeEgresoValido = false;
            }
            else
            {

                // Se obtiene el centro del destino de la carta de porte para validar el cupo de egreso
                int centroIdDestino = Convert.ToInt32(servicio.ObtenerConfiguracionGeneral(
                    Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass,
                    Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro).Valor);

                log.Debug("CentroIdDestino: {0}", centroIdDestino);

                var resultado = servicio.ValidarCupo(cartaPorteDto.CupoSalida, centroIdDestino, cartaPorteDto.NroCartaPorte);

                var validarCupoEnSAPResponse = this.ValidarCupoEnSAP(cartaPorteDto.CupoSalida, centroIdDestino , nameof(cartaPorteDto.CupoSalida));

                if (esCupoDeEgresoValido && resultado.YaAsignado)
                {
                    ModelState.AddModelError(nameof(cartaPorteDto.CupoSalida), resultado.MensajeError);
                    esCupoDeEgresoValido = false;
                }
                esCupoDeEgresoValido &= validarCupoEnSAPResponse.EsValido;
            }

            return esCupoDeEgresoValido;
        }

        #endregion

        /// <summary>
        /// Valida que la respuesta de SAP no sea nula y, de no serlo, además, que el cupo sea válido (siempre que no sea el 
        /// cupo genérico), así como también, la existencia del material del cupo de SAP en SCATO 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="cupo"></param>
        /// <param name="centroId"></param>
        /// <param name="respuestaSAP"></param>
        /// <param name="materialId">Parámetro de salida, distinto de cero sólo si es un cupo válido.</param>
        /// <returns>true, si es válido, de lo contrario false.</returns>
        private bool ValidarRespuestaSAP(string tipo, string cupo, int centroId, ZMPES5220 respuestaSAP, out int materialId)
        {
            bool esRespuestaSAPValida = true;
            materialId = 0;

            if (respuestaSAP != null)
            {
                log.Debug("ValidarRespuestaSAP {0}: {1}", respuestaSAP.CODIGO, respuestaSAP.ToXml());

                if (respuestaSAP.MENSAJE == Textos.RespuestaSap_NoValido)
                {
                    if (!respuestaSAP.CODIGO.Equals(Constantes.ValoresPorDefecto.CupoGenerico))
                    {
                        ModelState.AddModelError(tipo, respuestaSAP.MENSAJE);
                        esRespuestaSAPValida = false;
                    }
                }
                else
                {
                    materialId = servicio.ObtenerMaterialIdPorCodigoSap(respuestaSAP.MATERIAL.TrimStart(new[] { '0' }) ?? string.Empty);

                    if (materialId == 0)
                    {
                        ModelState.AddModelError(tipo, string.Format(Textos.Material_CodigoSAPNoExiste, respuestaSAP.MATERIAL));
                        esRespuestaSAPValida = false;
                    }
                }
                if (respuestaSAP.MENSAJE == Textos.RespuestaSap_NoValido)
                {
                    if (!respuestaSAP.CODIGO.Equals(Constantes.ValoresPorDefecto.CupoGenerico))
                    {
                        ModelState.AddModelError(tipo, respuestaSAP.MENSAJE);
                        esRespuestaSAPValida = false;
                    }
                }
            }
            else 
            {
                string mensaje = $"Cupo {cupo} no encontrado para el Centro {centroId}";
                log.Debug($"ValidarRespuestaSAP {mensaje}. La respuesta de SAP es null.");
                ModelState.AddModelError(tipo, mensaje);
                esRespuestaSAPValida = false;
            }

            return esRespuestaSAPValida;
        }

        #region -- ObtenerCuposPorCentrosDesdeSAP --

        public class CuposPorCentrosDesdeSAPRequest
        {
            public string Cupo { get; set; }

            public string[] CentrosSAP { get; set; }
        }

        public class CuposPorCentrosDesdeSAPResponse
        {
            public ZMPES5220 CupoSAP { get; set; }

            public bool EsEspecial { get; set; }
        }

        protected CuposPorCentrosDesdeSAPResponse ObtenerCuposPorCentrosDesdeSAP(CuposPorCentrosDesdeSAPRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.CentrosSAP == null || request.CentrosSAP.Length == 0)
                throw new ArgumentException("CentrosSAP no puede ser nulo ni vacío", nameof(request.CentrosSAP));

            // TestCases
            // 1) 1 único centro, 1 cupo válido                 ok
            // 2) 1 único centro, 1 cupo no válido              ok
            // 3) 2 centros, 1er cupo válido                    ok
            // 4) 2 centros, 1er cupo no válido, 2do válido     ok
            // 5) 2 centros, ambos cupos no válidos             ok

            CuposPorCentrosDesdeSAPResponse response = new CuposPorCentrosDesdeSAPResponse();

            var z2100Request = new Z_SDMF_RFC_Z2100Request
            {
                Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                {
                    IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = request.Cupo } },
                    IM_CENTRO = new ZMPES5210[] { new ZMPES5210() }
                }
            };

            int i = 0;

            do 
            {
                z2100Request.Z_SDMF_RFC_Z2100.IM_CENTRO[0].CENTRO = request.CentrosSAP[i];
                var z2100Response = servicioSap.Z_SDMF_RFC_Z2100(z2100Request);

                response.CupoSAP = z2100Response?.Z_SDMF_RFC_Z2100Response?.EX_CUPOS?.FirstOrDefault();
                response.EsEspecial = i > 0;

                i++;
            } 
            while (i < request.CentrosSAP.Length && response.CupoSAP.MENSAJE == Textos.RespuestaSap_NoValido);

            return response;
        }

        #endregion

        #region -- ValidarCupoEnSAP --

        public class ValidarCupoEnSAPResponse
        {
            public bool EsValido { get; set; }
            public int MaterialId { get; set; }
            public ZMPES5220 CupoSAP { get; set; }
            public bool EsEspecial { get; set; }
        }

        protected ValidarCupoEnSAPResponse ValidarCupoEnSAP(string cupo, int centroId , string keyForError)
        {
            var response = new ValidarCupoEnSAPResponse();

            try
            {
                string[] codigosDeCentroSAP = servicio.ObtenerCodigoDeCentroPorId(centroId);

                // Pueden existir hasta 2 códigos de centro en SAP, el principal y un especial
                if (codigosDeCentroSAP != null && codigosDeCentroSAP.Length > 0)
                {
                    log.Debug("ValidarCupoEnSAP: {0}, centros: {1}", cupo, string.Join(",", codigosDeCentroSAP));
                    var cuposPorCentrosDesdeSAPRequest = new CuposPorCentrosDesdeSAPRequest()
                    {
                        Cupo = cupo,
                        CentrosSAP = codigosDeCentroSAP
                    };
                    var cuposPorCentrosDesdeSAPResponse = this.ObtenerCuposPorCentrosDesdeSAP(cuposPorCentrosDesdeSAPRequest);

                    response.EsValido = this.ValidarRespuestaSAP(keyForError, cupo, centroId, cuposPorCentrosDesdeSAPResponse.CupoSAP, out int materialId);
                    response.MaterialId = materialId;
                    response.CupoSAP = cuposPorCentrosDesdeSAPResponse.CupoSAP;
                    response.EsEspecial = cuposPorCentrosDesdeSAPResponse.EsEspecial;
                }
                else
                {
                    string mensaje = $"No se encontraron códigos de centro SAP para el centro {centroId}";
                    log.Warn("ValidarCupoEnSAP " + mensaje);
                    ModelState.AddModelError(keyForError, mensaje);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al validar cupo de ingreso {cupo} para el centro {centroId}.");
                ModelState.AddModelError(keyForError, "Error al validar el cupo de ingreso");// Textos.Error_GenericoSap);
            }

            return response;
        }

        #endregion

        #endregion

        [DatosUsuario]
        protected virtual ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto { Actividad = Textos.ActCargarCartaPorte, ActividadXaml = "CargarCartaPorteByPass", PuestoDeTrabajoId = usuario.PuestoDeTrabajoId, NombreUsuario = usuario.NombreUsuario };
        }

        [DatosUsuario]
        public JsonResult ObtenerTipoVehiculoPorPatente(string patente, string acoplado, string workflow, DatosUsuario datosUsuario, string acoplado2 = "")
        {
            try
            {
                log.Debug("Obteniendo Tipo de vehiculo por patente {0} workflow {1}", patente, workflow);
                var resultadoEscalables = servicioComandos.Ejecutar(new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = acoplado2, Usuario = datosUsuario.NombreUsuario }) as ResultadoEscalables;

                log.Debug(resultadoEscalables.HayErrores ? "Error al obtener el tipo de vehiculo por patente{0}: " + resultadoEscalables.Errores.Values.First() : "Devolviendo el tipo de vehiculo por patente {0}", patente);

                return Json(new { resultadoEscalables.Categoria, CategoriaDesc = resultadoEscalables.Categoria != null ? resultadoEscalables.Categoria.DisplayText() : string.Empty, CodigoDeError = resultadoEscalables.HayErrores ? resultadoEscalables.Errores.Keys.First() : "0", Error = resultadoEscalables.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el tipo de vehiculo por patente {0}", patente);
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerImagenCpe(long nroCtg, DatosUsuario datosUsuario)
        {
            var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = nroCtg }) as ResultadoConsultarImagenCpe;

            if (cartaPorteImagen.HayErrores)
            {
                cartaPorteImagen = new ResultadoConsultarImagenCpe();
                var consultaAfip = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = nroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId }) as ResultadoCartaPorteElectronica;
                if (consultaAfip.HayErrores && consultaAfip.PdfImage is null)
                {
                    cartaPorteImagen.Errores.Add("3", "No se pudo obtener la imagen de la CP desde AFIP. Por favor intente nuevamente más tarde..");
                }
                else
                {
                    cartaPorteImagen.PdfImage = consultaAfip.PdfImage;
                }
            }

            return new JsonResult()
            {
                Data = new
                {
                    PdfImageBase64 = cartaPorteImagen.HayErrores ? string.Empty : Convert.ToBase64String(cartaPorteImagen.PdfImage),
                    CodigoDeError = cartaPorteImagen.HayErrores ? cartaPorteImagen.Errores.FirstOrDefault().Key : "0",
                    Error = cartaPorteImagen.HayErrores ? cartaPorteImagen.Errores.FirstOrDefault().Value : string.Empty
                },
                ContentType = "application/json",
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [DatosUsuario]
        public JsonResult ObtenerTipoVehiculo(string numero, string patente, string acoplado, string acoplado2, string workflow, DatosUsuario datosUsuario)
        {
            try
            {
                var vehiculo = servicio.BuscarCategoriaVehiculo(patente, acoplado, acoplado2);
                return vehiculo == null ? Json(new { CodigoDeError = 1, vehiculo }, JsonRequestBehavior.AllowGet) : Json(new { CodigoDeError = 0, vehiculo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el tipo de vehiculo por patente {0}", patente);
                throw;
            }
        }

        public ActionResult DescargarPDFCartaPorte(string id)
        {
            try
            {
                var cartaPorte = servicioComandos.Ejecutar(new ConsultarPDFCpe { NroCtg = Convert.ToInt64(id) }) as ResultadoConsultarPDFCpe;
                if (cartaPorte != null && !cartaPorte.HayErrores)
                    return File(cartaPorte.Pdf, "application/octet-stream", $"{id}.pdf");

                return null;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo descargar el PDF del CTG: {0}", id);
                throw;
            }
        }

        private string GuardarfotoMesaDigitalizacionSelloSustentable(CartaPorteDto orden, DatosUsuario datosUsuario, DateTime fecha, string numCtg = null)
        {
            if (!string.IsNullOrEmpty(orden.FotoRutaDestino))
            {
                log.Debug($"GuardarfotoMesaDigitalizacionSustentable {orden.FotoRutaDestino} {fecha}");
                var resultadoSustentable = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                {
                    RutaFotoCP = orden.FotoRutaDestino,
                    CodigoCentroSap = datosUsuario.CentroCodigoSap,
                    NroDocumento = orden.NroCartaPorte,
                    Patente = orden.Patente,
                    SoloDibujar = false
                }) as ResultadoGuardarFoto;
                return resultadoSustentable != null ? resultadoSustentable.Path : null;
            }
            return null;
        }

        private bool PatenteValida(string patente, TipoVehiculo tipo)
        {
            if (string.IsNullOrEmpty(patente)) return false;
            Regex patenteRegex = tipo == TipoVehiculo.Tren ? 
                new Regex(@"(^\d+$)") 
                : new Regex(@"(^[A-Z]{3}[0-9]{3}$)|(^[A-Z]{2}[0-9]{3}[A-Z]{2}$)");
            return patenteRegex.IsMatch(patente.ToUpper());
        }

        private bool ExisteExcepcionAlControlParaCartaPorte(IServicioRepositorio repositorio, int materialId, int transportistaId, int intermediarioId, int centroId, DateTime date, int? centroDestinoId, int? clienteDestinoId)
        {
            return intermediarioId > 0
                ? repositorio.ExisteExcepcionAlControlProveedorParaCartaPorte(materialId, intermediarioId, centroId, date, centroDestinoId, clienteDestinoId)
                : repositorio.ExisteExcepcionAlControlParaCartaPorte(materialId, transportistaId, centroId, date, centroDestinoId, clienteDestinoId);
        }

        #endregion
    }
}