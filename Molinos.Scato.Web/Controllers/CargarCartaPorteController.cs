using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
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

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadCargarCartaPorte)]
    public class CargarCartaPorteController : DocumentoIngresoController
    {
        private readonly IServicioActividadFactory<ICargarCartaPorteService> factory;
        private readonly IListaDeWorkflows workflows;
        protected readonly IFirmaProvider configuracion;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioOrquestador servicioOrquestador;

        public CargarCartaPorteController(ILogger log, IServicioRepositorio servicio, IServicioActividadFactory<ICargarCartaPorteService> factory, IServicioComandos servicioComandos, IListaDeWorkflows workflows, IFirmaProvider configuracion, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador)
            : base(log, servicio, servicioComandos)
        {
            this.configuracion = configuracion;
            this.factory = factory;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
            this.servicioOrquestador = servicioOrquestador;
        }
        
        [DatosUsuario]
        public virtual ActionResult Index(string workflow, DatosUsuario datosUsuario, string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "", int cargaDeCupoId = 0)
        {
            ViewBag.Usuario = datosUsuario.NombreUsuario;
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
            var cargaCupo = servicio.ObtenerCupoPorId(cargaDeCupoId);
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
                        var path = Path.GetDirectoryName(carga.FotoRutaDestino).Replace("temp","");
                        ViewBag.FotoMesaDigitalizacion1 = foto.Fotos.First().Foto;
                        ViewBag.PuestoDeTrabajo = path;
                    }
                }
                if (!(cargaCupo is null))
                {
                    var vehiculos = new List<VehiculoDto> { new VehiculoDto { Patente = cargaCupo.Patente } };
                    carta.Patente = cargaCupo.Patente;
                    carta.MaterialId = cargaCupo.MaterialId;
                    carta.Material = cargaCupo.MaterialDescripcion;
                    carta.VehiculoJson = vehiculos.ToJson();
                    carta.Cupo = cargaCupo.Cupo;
                }
            }
            ViewBag.aceptaPendiente = true;
            carta.FechaVto = DateTime.Now;
            return View(carta);
        }

        [HttpPost]
        [DatosUsuario]
        [ViewBagToResponseHeader]
        public virtual ActionResult Index(string workflow, string puestoDeTrabajo, string fotoMesaDigitalizacion1, string fotoMesaDigitalizacion2, CartaPorteDto orden, DatosUsuario datosUsuario)
        {
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;
            ViewBag.aceptaPendiente = true;

            if (string.IsNullOrEmpty(orden.NroCartaPorte) && orden.TipoVehiculoInt == (int)TipoVehiculo.Tren)
            {
                var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                orden.NroCartaPorte = $"{DateTime.Now.ToString("yyyyMMdd")}{sequenciaNroCartaPorteCPE.ToString("D4")}";
                ModelState.Remove("NroCartaPorte");
            }

            if (orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Egreso)
            {
                if (string.IsNullOrEmpty(orden.NroCartaPorte))
                {
                    var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                    orden.NroCartaPorte = $"{DateTime.Now.ToString("yyyyMMdd")}{sequenciaNroCartaPorteCPE.ToString("D4")}";
                }

                ModelState.Remove("NroCartaPorte");
                ModelState.Remove("CTG");
            }

            if(orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && servicio.ValidarCPERedespacho(orden.NumeroOperativo is null ? orden.NroCartaPorte : orden.NumeroOperativo.ToString(), datosUsuario.CentroId, workflow, orden.TipoVehiculoInt))
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
            if (ModelState.IsValid && vehiculos != null && vehiculos.Count() != 0)
            {
                if (servicio.TomaFotoEnMesa(datosUsuario.CentroId))
                {
                    if (String.IsNullOrEmpty(fotoMesaDigitalizacion1))
                    {
                        log.Debug("Foto de CP 1 es requerida: {1}", orden.NroCartaPorte);
                        ModelState.AddModelError("", Textos.Error_FotoCP);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                    if (vehiculos.First().TipoVehiculo == TipoVehiculo.Tren && !orden.Cpe && String.IsNullOrEmpty(fotoMesaDigitalizacion2))
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

                if (vehiculos.Count() != vehiculos.GroupBy(x => x.Patente).Count())
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

                if (tipoComercial?.PesoMaximoDocumentoIngreso != null && tipoComercial?.PesoMaximoDocumentoIngreso != 0)
                {
                    if (vehiculos.Any(vehiculo => vehiculo.PesoBrutoOrigen > tipoComercial.PesoMaximoDocumentoIngreso))
                    {
                        log.Debug("El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                        ModelState.AddModelError("", Textos.OrdenCargaInterna_PesoMaximoExcedido);
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                }

                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    log.Debug("No se pudo dar de alta o asociar el chofer a la CP");
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    log.Debug("No se pudo dar de alta o asociar el transportista a la CP");
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
                }

                if (orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && orden.Cpe && orden.TransportistaTramo2Id != 0)
                {
                    var transportistaTramo2Id = orden.TransportistaTramo2Id ?? 0;
                    var resultadoTransportistaTramo2 = SetearTransportista(ref transportistaTramo2Id, orden.TipoComercialId, orden.EsTransportistaTramo2);
                    orden.TransportistaTramo2Id = transportistaTramo2Id;

                    if (!resultadoTransportistaTramo2)
                    {
                        log.Debug("No se pudo dar de alta o asociar el transportista2 a la CP");
                        SetearVista(workflowObj, datosUsuario.CentroId);
                        return View(orden);
                    }
                }

                if (!ValidarCupo(orden, datosUsuario, workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso))
                {
                    SetearVista(workflowObj, datosUsuario.CentroId);
                    return View(orden);
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
                    vehiculo.Patente = vehiculo.Patente != null ? vehiculo.Patente.ToUpper() : "";
                    vehiculo.PatenteAcoplado = vehiculo.PatenteAcoplado != null ? vehiculo.PatenteAcoplado.ToUpper() : "";
                    vehiculo.PatenteAcoplado2 = vehiculo.PatenteAcoplado2 != null ? vehiculo.PatenteAcoplado2.ToUpper() : "";
                    vehiculo.NumeroVehiculo = i++;
                }
                var fecha = DateTime.Now;
                orden.FotoRutaDestino = GuardarfotoMesaDigitalizacion(fotoMesaDigitalizacion1, orden, puestoDeTrabajo, datosUsuario, fecha);
                
                if (!String.IsNullOrEmpty(fotoMesaDigitalizacion2))
                {
                    orden.FotoRutaDestinoDetalle = GuardarfotoMesaDigitalizacion(fotoMesaDigitalizacion2, orden, puestoDeTrabajo, datosUsuario, fecha.AddMinutes(1));
                }
                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var instanceIds = new List<Guid>();

                log.Info("CargarCartaPorte: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                foreach (var vehiculo in vehiculos)
                {
                    if(orden.TipoVehiculoInt == (int)TipoVehiculo.Tren && orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
                    {
                        var ctgVagon = Convert.ToInt64(string.IsNullOrEmpty(vehiculo?.NumCTG) ? orden.NroCartaPorte : vehiculo?.NumCTG);
                        var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = ctgVagon }) as ResultadoConsultarImagenCpe;                        
                        if (!cartaPorteImagen.HayErrores)
                        {
                            var imagenBase64 = Convert.ToBase64String(cartaPorteImagen.PdfImage);
                            orden.FotoRutaDestino = GuardarfotoMesaDigitalizacion(imagenBase64, orden, puestoDeTrabajo, datosUsuario, fecha, ctgVagon.ToString());
                        }
                    }
                    var controlRecorrido = GenerarControlRecorrido(datosUsuario);
                    var resultadoActividad = servicioWf.CargarCartaPorte(orden, vehiculo, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
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
                        Mensaje = string.Format("El camión ha sido demorado por el siguiente motivo: {0}", orden.MotivoDemora),
                        SePuedeDesactivar = false
                    });
                }

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
                return View("~/Views/CargarCartaPorte/Camion.cshtml");
            }

            var modelo = model.FromJson<CartaPorteDto>();
            return View("~/Views/CargarCartaPorte/Camion.cshtml", modelo);
        }

        public ActionResult MostrarVagonesJson(string model)
        {
            if (String.IsNullOrEmpty(model))
            {
                return View("~/Views/CargarCartaPorte/Vagon.cshtml");
            }
            var modelo = model.FromJson<CartaPorteDto>();
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
                log.Error(e, "No se pudo obtener la carta de porte CTG {0}", numeroCtg);
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerCartaPorteCtgCompraGranos(long numeroCtg, string workflow, int tipoVehiculo, bool consultactg, DatosUsuario datosUsuario)
        {
            try
            {
                if (!servicio.ValidarCPERedespacho(numeroCtg.ToString(), datosUsuario.CentroId, workflow, tipoVehiculo, consultactg))
                {
                    log.Debug("Obteniendo CTG {0} workflow {1}", numeroCtg, workflow);
                    if (tipoVehiculo != (int)TipoVehiculo.Tren)
                    {
                        var cartaPorteResponseDB = servicio.ObtenerCartaPorteAReutilizarPorNumero(numeroCtg.ToString(), datosUsuario.CentroId, workflow);
                        if (cartaPorteResponseDB.CodigoDeError != 1)
                        {
                            var vehiculo = cartaPorteResponseDB.CartaPorte.Vehiculos.FirstOrDefault();
                            var categoriaVehiculo = servicio.BuscarCategoriaVehiculo(vehiculo.Patente, vehiculo.PatenteAcoplado, vehiculo.PatenteAcoplado2);
                            if(categoriaVehiculo != null)
                            {
                                cartaPorteResponseDB.CartaPorte.TipoVehiculo = (Dominio.Enums.TipoVehiculo)categoriaVehiculo.TipoVehiculo;
                            }
                            return Json(new { Cpe = cartaPorteResponseDB.CartaPorte, cartaPorteResponseDB.CodigoDeError, cartaPorteResponseDB.Error }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var cartaPorteResponse = new ResultadoCartaPorteElectronica();
                    cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = numeroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId, TipoVehiculo = tipoVehiculo, ConsultaFerroviarioPorCtg = consultactg }) as ResultadoCartaPorteElectronica;

                    var errorCode = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "3";
                    var errorMsg = cartaPorteResponse.Errores.Values.FirstOrDefault();
                    var estadoPermiteIngresar = new List<string> { "AC", "CF", "CO" };

                    if (!cartaPorteResponse.HayErrores && !estadoPermiteIngresar.Contains(cartaPorteResponse.Cpe.EstadoCpe))
                    {
                        if (new List<string> { "AN", "RE" }.Any(a => a == cartaPorteResponse.Cpe?.EstadoCpe))
                        {
                            errorMsg = $"El CTG {numeroCtg} se encuentra en estado {(cartaPorteResponse.Cpe?.EstadoCpe?.ToUpper()?.Trim() == "AN" ? "ANULADO" : "RECHAZADO")}";
                            errorCode = "5";
                        }
                        else
                        {
                            errorMsg = string.Format("El CTG {0} no se encuentra en estado ACTIVO", numeroCtg);
                            errorCode = "4";
                        }
                    }

                    log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener carta de porte CTG-CPE {0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo carta de porte CTG-CPE {0}", numeroCtg);
                    return Json(new { cartaPorteResponse.Cpe, CodigoDeError = errorCode, Error = errorMsg }, JsonRequestBehavior.AllowGet);
                } else
                {
                    return Json(new { CodigoDeError = "2", Error = "No existen solicitudes para los parámetros indicados." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte CTG-CPE {0}", numeroCtg);
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
        }

        protected virtual CartaPorteValidaResponseDto ValidarNumeroCartaPorte(CartaPorteDto orden, int centroId, string workflowCodigo)
        {
            return servicio.NumeroCartaPorteValido(orden.NroCartaPorte, centroId, workflowCodigo, orden.Cpe);
        }

        protected virtual bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            var codigoSapMolinosAgro = configuracion.ObtenerFirmaSinLogo().CodigoSAP;
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId).CodigoSap;
            var codigoDeEstablecimiento = orden.CodEstab;
            var remitente = servicio.ObtenerProveedor(orden.RtteComercialId);
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);

            var codigoEstablecimientoEsDeMolinos = servicio.ObtenerCodigoEstablecimientoEsDeMolinos(codigoDeEstablecimiento);

            //si es MRP, no se valida el codigo de establecimiento
            if (codigoSapTitular == codigoSapMRP && (remitente == null || remitente.CodigoSap == codigoSapMRP || remitente.CodigoSap == codigoSapMolinosAgro))
            {
                ModelState.AddModelError("", Textos.Error_CCPPCompra);
                return false;
            }
            else if ((codigoSapTitular == codigoSapMolinosAgro) && (remitente == null || (remitente.CodigoSap == codigoSapMolinosAgro)) && codigoEstablecimientoEsDeMolinos)
            {
                ModelState.AddModelError("", Textos.Error_CCPPCompra);
                return false;
            }
            if (otroRecorridoDelChofer != null && !(orden.TipoVehiculo == TipoVehiculo.Tren))
            {
                ModelState.AddModelError("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return false;
            }
            return true;
        }

        protected virtual bool ValidarCupo(CartaPorteDto orden, DatosUsuario usuario, bool esIngreso)
        {
            //No validamos Si no requiere cupo o si el destino no es un centro de MOA
            if (!orden.RequiereCupo || !orden.ValidarCupo || (!esIngreso && orden.EsClienteDestinatario) || (!esIngreso && !orden.Cupo.StartsWith("MOL")))
            {
                return true;
            }
            var resultado = servicio.ValidarCupo(orden.Cupo, usuario.CentroId, orden.NroCartaPorte);
            if(resultado.Reingresado != null)
            {
                var resultadoCargaDeCupo = servicioComandos.Ejecutar(
                    new CrearCargaDeCupo
                    {
                        Dto = resultado.Reingresado
                    });
                if (resultadoCargaDeCupo.HayErrores)
                {
                    ModelState.AddModelError("Cupo", resultadoCargaDeCupo.Errores.First().Value);
                    return false;
                }
            }
            if (resultado.Valido && !resultado.YaAsignado)
            {
                return true;
            }

            if (resultado.YaAsignado)
            {
                ModelState.AddModelError("Cupo", resultado.MensajeError);
                return false;
            }

            //if (servicio.ValidarCupoCartaPorte(orden.Cupo, usuario.CentroId, orden.NroCartaPorte))
            //{
            //    ModelState.AddModelError("Cupo", "El cupo fue ingresado con otra CP");
            //    return false;
            //}

            return ValidarCupoEnSap(orden, usuario, esIngreso);
        }


        private bool ValidarCupoEnSap(CartaPorteDto orden, DatosUsuario datosUsuario, bool esIngreso)
        {
            try
            {
                var centroDelCupoId = esIngreso ? datosUsuario.CentroId : orden.DestinoId;
                var codigosDeCentroSap = servicio.ObtenerCodigoDeCentroPorId(centroDelCupoId);
                log.Debug("ValidarCupoEnSap cupo: {0}, centro: {1}", orden.Cupo, string.Join(",", codigosDeCentroSap));
                var esEspecial = false;
                var response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                {
                    Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                    {
                        IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[0] } },
                        IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = orden.Cupo } }
                    }
                });

                var respuesta = response.Z_SDMF_RFC_Z2100Response.EX_CUPOS.FirstOrDefault();

                if (respuesta != null && respuesta.MENSAJE == Textos.RespuestaSap_NoValido && codigosDeCentroSap.Length > 1)
                {
                    response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                    {
                        Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                        {
                            IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[1] } },
                            IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = orden.Cupo } }
                        }
                    });
                    respuesta = response.Z_SDMF_RFC_Z2100Response.EX_CUPOS.FirstOrDefault();
                    esEspecial = true;
                }

                if (respuesta != null && respuesta.MENSAJE != Textos.RespuestaSap_NoValido)
                {
                    log.Debug("ValidarCupoEnSap Respuesta {0}: {1}", orden.Cupo, respuesta.ToXml());
                    var materialId = servicio.ObtenerMaterialIdPorCodigoSap(respuesta.MATERIAL.TrimStart(new[] { '0' }));
                    if (materialId == 0)
                    {
                        ModelState.AddModelError("Cupo", string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL));
                        return false;
                    }

                    var resultadoCargaDeCupo = servicioComandos.Ejecutar(
                    new CrearCargaDeCupo
                    {
                        Dto = new CargaDeCupoDto
                        {
                            CentroId = datosUsuario.CentroId,
                            Cupo = orden.Cupo,
                            Fecha = DateTime.Now,
                            FechaSap = respuesta.FECHA,
                            MaterialId = materialId,
                            PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                            RespuestaSap = respuesta.MENSAJE,
                            Camara = respuesta.CALIDAD,
                            Especial = esEspecial
                        }
                    });
                    if (resultadoCargaDeCupo.HayErrores)
                    {
                        ModelState.AgregarErrores(resultadoCargaDeCupo);
                        return false;
                    }
                    return true;
                }
                log.Debug("ValidarCupoEnSap Respuesta {0} no encontrado", orden.Cupo);
                ModelState.AddModelError("Cupo", respuesta.MENSAJE);
                return false;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al validar cupo en SAP: ");
                ModelState.AddModelError("Cupo", Textos.Error_GenericoSap);
                return false;
            }
        }

        [DatosUsuario]
        protected virtual ControlRecorridoDto GenerarControlRecorrido(DatosUsuario usuario)
        {
            return new ControlRecorridoDto { Actividad = Textos.ActCargarCartaPorte, ActividadXaml = "CargarCartaPorte", PuestoDeTrabajoId = usuario.PuestoDeTrabajoId, NombreUsuario = usuario.NombreUsuario };
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
        public JsonResult ObtenerTipoVehiculoPorPatenteCPE(long nroCtg, long cuit, DatosUsuario datosUsuario)
        {
            try
            {
                var cartaPorte = (ResultadoCartaPorteElectronica) servicioComandos.Ejecutar(new ConsultarCPDigital {CuitSolicitante = cuit, NroCtg = nroCtg });

                return Json(new { cartaPorte }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo obtener cpe para ctg { nroCtg } y cuit { cuit }");
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
                var consultaAfip = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = nroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId, ConsultaImagenCpe = true }) as ResultadoCartaPorteElectronica;
                if(consultaAfip.HayErrores && consultaAfip.PdfImage is null)
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
                Data = new { 
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
                if(cartaPorte != null && !cartaPorte.HayErrores)
                    return File(cartaPorte.Pdf, "application/octet-stream", $"{id}.pdf");

                return null;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo descargar el PDF del CTG: {0}", id);
                throw;
            }
        }
    }
}