using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.CargaDeCupo)]
    public class CargaDeCupoController : BaseController
    {
        private ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows workflows;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IConfiguracionProvider configuracion;
        private readonly IFirmaProvider firma;
        private readonly IServicioActividadFactory<ICargarCartaPorteService> factory;

        public CargaDeCupoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, 
            IListaDeWorkflows workflows, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador, 
            IConfiguracionProvider configuracion, IFirmaProvider firma,
            IServicioActividadFactory<ICargarCartaPorteService> factory)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
            this.servicioOrquestador = servicioOrquestador;
            this.firma = firma;
            this.factory = factory;
            this.configuracion = configuracion;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(CargaDeCupoDto model, string imagenCartaPorte, bool AvanceCpe, DatosUsuario datosUsuario)
        {
            if (model.CircuitoNoGranos)
            {
                return RedirectToAction("IndexNoGranos", model);
            }
            model.ImagenCartaPorte = imagenCartaPorte.Replace("data:image/jpg;base64,", "");
            ViewBag.Materiales = servicio.ListarMaterialesPorWorkflow(225, datosUsuario.CentroId).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
            ViewBag.AvanzaAutomatico = centro.AvanzaCpe;
            if (centro.AvanzaCpe != AvanceCpe)
            {
                centro.AvanzaCpe = AvanceCpe;
                servicioComandos.Ejecutar(new ModificarCentro { Dto = centro, Usuario = datosUsuario.NombreUsuario });
            }

            ModelState.Remove("MaterialId");
            ModelState.Remove("Especial");
            if (model.CPE)
            {
                ModelState.Remove("NumeroCartaPorte");
            }
            else
            {
                ModelState.Remove("CTG");
            }

            if (ModelState.IsValid)
            {
                log.Debug("Asignación de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                if (string.IsNullOrEmpty(model.ImagenCartaPorte) && !model.SinFotoCartaPorte && !model.CPE)
                {
                    ModelState.AddModelError("", string.Format(Textos.Error_Requerido, "Foto de CP"));
                    log.Debug("ERROR 1 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }
                if (servicio.EsTarjetaBloqueada(model.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("", Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);
                    log.Debug("ERROR 2 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }
                if (!servicio.EsTarjetaEnRangoValido(model.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("", Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);
                    log.Debug("ERROR 3 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }
                var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(model.Numero, datosUsuario.CentroId);
                if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
                {
                    ModelState.AddModelError("", Textos.ImpresionTarjetaDeAcceso_EnUso);
                    log.Debug("ERROR 4 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }

                var validarTarjetaEnUsoPendienteSinRecorrido = ConfigurationManager.AppSettings["ValidarTarjetaEnUsoEtapaPendiente"];
                if (!string.IsNullOrEmpty(validarTarjetaEnUsoPendienteSinRecorrido) && validarTarjetaEnUsoPendienteSinRecorrido == "1")
                {
                    var intanciaWorkflow = workflows.ObtenerWorkflowPendientePorNumeroTarjetaAcceso(model?.Numero, datosUsuario?.CentroId);
                    if (!(intanciaWorkflow is null))
                    {
                        ModelState.AddModelError("", string.Format(Textos.TarjetaDeAcceso_EnUso_Pendiente, intanciaWorkflow.Patente));
                        return View("Form", model);
                    }
                }

                model.Fecha = DateTime.Now;
                model.CentroId = datosUsuario.CentroId;
                model.CentroCodigoSap = datosUsuario.CentroCodigoSap;
                model.Patente = model.Patente.ToUpper();
                //var resultado = servicioComandos.Ejecutar(new CrearCargaDeCupo { Dto = model, EsGarita = true }) as ResultadoCrear;
                var resultado = servicioComandos.Ejecutar(new CrearCargaDeCupo { Dto = model, EsGarita = true }) as ResultadoCrear;
                if (resultado.HayErrores)
                {
                    log.Debug("ERROR 5 de cupo {0}, tarjeta {1}, centro {2}, CP {3}: " + resultado.Errores.First().Value, model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    foreach (var r in resultado.Errores)
                    {
                        ModelState.AddModelError("", r.Value);
                    }
                }
                else
                {
                    model.FotoRutaDestino = resultado.Mensaje;
                    model.FotoRutaSustentable = resultado.PathSustentable;

                    if (!model.NoAsignaCalleEnGaritaEntrada)
                    {
                        log.Debug("Asignar Calle: Resultado Id= {0}, Patente: {1}, MaterialId: {2}", resultado.Id, model.Patente, model.MaterialId);
                        var turnoActivo = InformarArribo(model.CPE ? model.CTG : model.NumeroCartaPorte, datosUsuario.CentroId, model.Patente, model.MaterialId);
                        var codigoBarrera = servicio.ObtenerDispositivoBarreraEntrada(model.PuestoDeTrabajoId);
                        AsignarCalle(resultado.Id, turnoActivo, model.CPE ? model.CTG : model.NumeroCartaPorte, datosUsuario.CentroId, datosUsuario.NombrePc, model.Patente);
                        log.Info($"Ejecutando Apertura Barrera Garita con CodigoBarrera : { codigoBarrera} y Patente : {model.Patente}");
                        AperturaDeBarrera(codigoBarrera);
                    }
                    if (model.ImprimeTarjetaDeAcceso)
                    {
                        ImprimirTarjetaDeAcceso(model, datosUsuario, resultado);
                    }
                }

                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    ViewBag.MostrarAlertaExitosa = true;
                    if (AvanceCpe)
                        CargarCartaPorte(resultado.Id, datosUsuario);
                    return View("Form");
                }
            }
            return View("Form", model);
        }

        [DatosUsuario]
        public ActionResult IndexNoGranos(CargaDeCupoDto model, DatosUsuario datosUsuario)
        {
            model.Patente = model.Patente.ToUpper();
            ViewBag.Materiales = servicio.ListarMaterialGranoPorCentro(datosUsuario.CentroId, model.CircuitoNoGranos)
                .ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            var centro = servicio.ObtenerCentro(datosUsuario.CentroId);
            ViewBag.AvanzaAutomatico = centro.AvanzaCpe;
            ModelState.Remove("NumeroCartaPorte");
            ModelState.Remove("CTG");
            if (ModelState.IsValid)
            {
                log.Debug("Asignación de Cupo No Granos {0}, tarjeta {1}, centro {2}", model.Cupo, model.Numero, datosUsuario.CentroId);
                if (servicio.EsTarjetaBloqueada(model.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("", Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);
                    log.Debug("ERROR 2 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }
                if (!servicio.EsTarjetaEnRangoValido(model.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("", Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);
                    log.Debug("ERROR 3 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }
                var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(model.Numero, datosUsuario.CentroId);
                if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
                {
                    ModelState.AddModelError("", Textos.ImpresionTarjetaDeAcceso_EnUso);
                    log.Debug("ERROR 4 de cupo {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    return View("Form", model);
                }

                var validarTarjetaEnUsoPendienteSinRecorrido = ConfigurationManager.AppSettings["ValidarTarjetaEnUsoEtapaPendiente"];
                if (!string.IsNullOrEmpty(validarTarjetaEnUsoPendienteSinRecorrido) && validarTarjetaEnUsoPendienteSinRecorrido == "1")
                {
                    var intanciaWorkflow = workflows.ObtenerWorkflowPendientePorNumeroTarjetaAcceso(model?.Numero, datosUsuario?.CentroId);
                    if (!(intanciaWorkflow is null))
                    {
                        ModelState.AddModelError("", string.Format(Textos.TarjetaDeAcceso_EnUso_Pendiente, intanciaWorkflow.Patente));
                        return View("Form", model);
                    }
                }

                model.Fecha = DateTime.Now;
                model.CentroId = datosUsuario.CentroId;
                model.CentroCodigoSap = datosUsuario.CentroCodigoSap;
                var resultado = servicioComandos.Ejecutar(new CrearCargaDeCupoNoGrano { Dto = model }) as ResultadoCrear;

                if (resultado.HayErrores)
                {
                    log.Debug("ERROR 5 de cupo {0}, tarjeta {1}, centro {2}, CP {3}: " + resultado.Errores.First().Value, model.Cupo, model.Numero, datosUsuario.CentroId, model.NumeroCartaPorte);
                    foreach (var r in resultado.Errores)
                    {
                        ModelState.AddModelError("", r.Value);
                    }
                }
                else
                {
                    var codigoBarrera = servicio.ObtenerDispositivoBarreraEntrada(model.PuestoDeTrabajoId);
                    if (!model.NoAsignaCalleEnGaritaEntrada && model.MaterialId != 0)
                    {
                        log.Debug("Asignar Calle: Resultado Id= {0}, Patente: {1}, MaterialId: {2}", resultado.Id, model.Patente, model.MaterialId);
                        var turnoActivo = InformarArribo(model.NumeroCartaPorte, datosUsuario.CentroId, model.Patente, model.MaterialId);
                        AsignarCalle(resultado.Id, turnoActivo, model.NumeroCartaPorte, datosUsuario.CentroId, datosUsuario.NombrePc, model.Patente, true);
                    }
                    if (!model.NoAsignaCalleEnGaritaEntrada && model.MaterialId == 0 && ModelState.IsValid)
                    {
                        MostrarPorCartel(datosUsuario.NombrePc, "Mesa FAS", datosUsuario.CentroId, model.Patente);
                        ViewBag.EsCircuitoNoGranosSinMaterial = true;
                    }
                    if (model.ImprimeTarjetaDeAcceso)
                    {
                        ImprimirTarjetaDeAcceso(model, datosUsuario, resultado);
                    }

                    if (!model.NoAsignaCalleEnGaritaEntrada && model.MaterialId != 0)
                    {
                        model.MaterialId = 0;
                    }

                    log.Info($"Ejecutando Apertura Barrera Garita con CodigoBarrera : { codigoBarrera} y Patente : {model.Patente}");
                    AperturaDeBarrera(codigoBarrera);
                }

                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    ViewBag.MostrarAlertaExitosa = true;
                    return View("Form");
                }
            }
            return View("Form", model);
        }

        private void AsignarCalle(int cargaDeCupoId, bool turnoActivo, string cartaPorte, int centroId, string nombrePc, string patente, bool circuitoNoGranos = false)
        {
            try
            {
                var resultado = servicioComandos.Ejecutar(new CrearCallePorRecorrido
                {
                    TipoCalle = circuitoNoGranos ? TipoCalle.NoGranos : TipoCalle.PreCalado,
                    CargaDeCupoId = cargaDeCupoId,
                    TurnoActivo = turnoActivo,
                    CentroId = centroId
                });
                if (resultado.HayErrores)
                {
                    ModelState.AddModelError("warning", resultado.Errores.Values.First());
                }
                else
                {
                    var resultadoCrear = resultado as ResultadoCrearCalle;
                    if (resultadoCrear != null)
                    {
                        var fila = servicio.ObtenerCalleNombre(resultadoCrear.Id);
                        ViewBag.Disponibilidad = resultadoCrear.Disponibilidad;
                        ViewBag.FilaAsignadaNombre = $"{fila}";

                        if (!circuitoNoGranos)
                        {
                            servicioComandos.Ejecutar(new EnviarMensajeCamioneroCircular
                            {
                                CartaPorte = cartaPorte,
                                Mensaje = string.Format("Por favor avanzar, ubicarse en la \"{0}\" y espere a ser llamado para calado", fila),
                                SePuedeDesactivar = true
                            });
                        }
                        log.Debug($"Fila asignada {fila} por el puestoId: {nombrePc}");
                        MostrarPorCartel(nombrePc, fila, centroId, patente);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "AsignarCalle");
            }
        }

        private bool InformarArribo(string cartaPorte, int centroId, string patente, int materialId)
        {
            var responseCircular = servicioComandos.Ejecutar(new InformarArriboCircular
            {
                CentroId = centroId,
                CartaPorte = cartaPorte,
                Patente = patente,
                MaterialId = materialId
            });

            var resp = responseCircular as ResultadoCircular;

            if (resp.HayErrores)
            {
                log.Error(resp.Errores.Values.First());
            }
            return resp.TurnoActivo;
        }

        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario)
        {
            ViewBag.Items = servicio.ListarDatosDeWorkflowsPendientes(datosUsuario.CentroId, 5);
            return View();
        }

        [DatosUsuario]
        public JsonResult ValidarCupoEnSap(string cupo, string imagen, DatosUsuario datosUsuario)
        {
            var model = new CargaDeCupoDto();
            var pdfSustentableString = string.Empty;

            //model.MaterialId = 4;
            //model.RespuestaSap = "Cupo del día";
            //model.ProveedorDescripcion = "MOLINOS AGRO S.A.";
            //model.ProveedorCuit = "30-71511877-3";
            //model.MaterialDescripcion = "Poroto de soja";
            //model.FechaSap = "2021-02-28";
            //model.Especial = true;
            //model.Camara = "Fabrica";
            //return Json(new { model, PdfImageSustentableBase64 = pdfSustentableString }, JsonRequestBehavior.AllowGet);

            if (servicio.CupoConsumido(cupo, datosUsuario.CentroId))
            {
                return Json(new { error = Textos.CupoConsumido }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var codigosDeCentroSap = servicio.ObtenerCodigoDeCentroPorId(datosUsuario.CentroId);
                    log.Debug("ValidarCupoEnSap cupo: {0}, centro: {1}", cupo, string.Join(",", codigosDeCentroSap));
                    var esEspecial = false;
                    var response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                    {
                        Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                        {
                            IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[0] } },
                            IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = cupo } }
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
                                IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = cupo } }
                            }
                        });
                        respuesta = response.Z_SDMF_RFC_Z2100Response.EX_CUPOS.FirstOrDefault();
                        esEspecial = true;
                    }

                    if (respuesta != null && respuesta.MENSAJE != Textos.RespuestaSap_NoValido)
                    {
                        log.Debug("ValidarCupoEnSap Respuesta {0}: {1}", cupo, respuesta.ToXml());
                        var material = servicio.ObtenerMaterialIdYDescripcionPorCodigoSap(respuesta.MATERIAL.TrimStart(new[] { '0' }));
                        if (material.MaterialId == 0)
                        {
                            ModelState.AddModelError("Cupo", string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL));
                            return Json(new { error = string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL) }, JsonRequestBehavior.AllowGet);
                        }
                        model.MaterialId = material.MaterialId;
                        model.RespuestaSap = respuesta.MENSAJE;
                        model.ProveedorDescripcion = respuesta.DESCPROV;
                        model.ProveedorCuit = respuesta.CUIT;
                        model.MaterialDescripcion = material.Descripcion;
                        model.FechaSap = respuesta.FECHA;
                        model.Especial = esEspecial;
                        model.Camara = respuesta.CALIDAD;

                        if (model.Especial && !String.IsNullOrEmpty(imagen))
                        {
                            imagen = imagen.Replace("data:image/jpg;base64,", string.Empty);
                            var imagenSustentable = Convert.FromBase64String(imagen);
                            imagenSustentable = DibujarSelloSustentable(imagenSustentable);
                            if (imagenSustentable != null)
                            {
                                pdfSustentableString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imagenSustentable));
                            }
                        }
                        return Json(new { model, PdfImageSustentableBase64 = pdfSustentableString }, JsonRequestBehavior.AllowGet);
                    }
                    log.Debug("ValidarCupoEnSap Respuesta {0} no encontrado", cupo);
                    return Json(new { error = respuesta.MENSAJE }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    log.Error(e, "Error al validar cupo en SAP: ");
                    return Json(new { error = Textos.Error_GenericoSap }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerCupoCtg(string numeroCartaPorte, string workflow, DatosUsuario datosUsuario)
        {
            ////return Json(new { CartaPorte = new { NroCartaPorte = "000111111111", Cupo = "MOL3333/29012020", Patente = "CCH873", CTG = "11111111", TitularCartaPorteCodigoSap = "200069" }, CodigoDeError = "0", CodEstab = "1234" }, JsonRequestBehavior.AllowGet);
            try
            {
                log.Debug("Obteniendo CUPO por CP {0} workflow {1}", numeroCartaPorte, workflow);

                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCupoCTG { CentroId = datosUsuario.CentroId, NumeroCartaPorte = numeroCartaPorte, Usuario = datosUsuario.NombreUsuario }) as ResultadoDetalleCTG;
                log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener Cupo CTG{0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo Cupo por CP {0}", numeroCartaPorte);

                if (cartaPorteResponse.HayErrores)
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "0", Error = cartaPorteResponse.Errores.Values.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
                }

                var inhabilitacion = servicio.ListarInhabilitacionCamion(cartaPorteResponse.CartaPorte.Patente, datosUsuario.CentroId);
                if (inhabilitacion.Any())
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "3", Error = inhabilitacion }, JsonRequestBehavior.AllowGet);
                }

                var validarCupo = servicio.ValidarCupo(cartaPorteResponse.CartaPorte.Cupo, datosUsuario.CentroId, cartaPorteResponse.CartaPorte.NroCartaPorte);
                if (validarCupo.Valido && validarCupo.YaAsignado)
                {
                    return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "2", Error = "El cupo ya esta asignado a otra CP" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { cartaPorteResponse.CartaPorte, CodigoDeError = "0" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el Cupo CP {0}", numeroCartaPorte);
                throw;
            }
        }

        [DatosUsuario]
        public JsonResult ObtenerFoto(string puestodetrabajoid, string codigoCamara, string directorio, CargaDeCupoDto model, bool fotoPatente = true)
        {
            if (string.IsNullOrEmpty(codigoCamara) || string.IsNullOrEmpty(directorio))
            {
                try
                {
                    var videocamara = ObtenerVideoCamaraPorPuesto(puestodetrabajoid, fotoPatente);
                    codigoCamara = videocamara.Codigo;
                    directorio = videocamara.Directorio;
                }
                catch (Exception e)
                {
                    return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                //Se asume que la primera cámara apunta al frente del camión y la última a la CP
                var resultadoEjecutar = servicioOrquestador.Ejecutar(new EjecutarTomarFoto
                {
                    CodigoDispositivo = codigoCamara
                });

                var resultado = resultadoEjecutar as ResultadoObtenerPatente;
                var resultadoTomarFoto = resultadoEjecutar as ResultadoTomarFoto;
                if (resultado != null)
                {
                    if (!fotoPatente && model != null)
                    {
                        resultado.Imagen = DibujarEtiqueta(resultado.Imagen, model);
                    }
                    return Json(new
                    {
                        imagen = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(resultado.Imagen)),
                        patente = resultado.Patente,
                        confianza = resultado.Confianza,
                        error = "",
                        directorio = directorio
                    }, JsonRequestBehavior.AllowGet);
                }
                if (resultadoTomarFoto != null)
                {
                    if (!fotoPatente && model != null)
                    {
                        resultadoTomarFoto.Imagen = DibujarEtiqueta(resultadoTomarFoto.Imagen, model);
                    }
                    return Json(new
                    {
                        imagen = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(resultadoTomarFoto.Imagen)),
                        error = "",
                        directorio = directorio
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { error = "No se pudo obtener la imagen" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = "No se pudo obtener la imagen" }, JsonRequestBehavior.AllowGet);
        }

        private void ImprimirDeCartaPorte(CargaDeCupoDto model, ResultadoCrear resultado)
        {
            if (!ModelState.IsValid) return;
            var codigo = "ImpresionCartaPorteMesa";
            var documento = servicio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, model.CentroId, model.PuestoDeTrabajoId);
            if (documento == null)
            {
                ModelState.AddModelError("Imp", String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo));
                return;
            }

            var resultadoImpresion = servicioComandos.Ejecutar(new ImprimirCartaPorteMesa
            {
                Dto = new ImpCartaPorteUrenportDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    FechaImpresion = DateTime.Now,
                    WorkflowId = new Guid(),
                    Patente = "",
                    FotoRutaDestino = model.FotoRutaDestino,
                }
            });
            if (resultadoImpresion.HayErrores)
            {
                servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = resultado.Id });
                ModelState.AddModelError("Imp", Textos.ErrorImpresionCpMesa + resultadoImpresion.Errores.First().Value);
            }
        }

        private void ImprimirTarjetaDeAcceso(CargaDeCupoDto model, DatosUsuario datosUsuario, ResultadoCrear resultado)
        {
            if (!ModelState.IsValid) return;

            var resultadoImpresion = servicioComandos.Ejecutar(new ImprimirTarjetaDeAcceso
            {
                Dto = new ImpTarjetaDeAccesoDto
                {
                    Codigo = "ImpresionTarjetaDeAcceso",
                    Numero = model.Numero,
                    Fecha = DateTime.Now.Formatted(),
                    CentroId = datosUsuario.CentroId,
                    PuestoDeTrabajoId = model.PuestoDeTrabajoId
                },
                OrigenImpresion = "CargaDeCupoController"
            });
            if (resultadoImpresion.HayErrores)
            {
                servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = resultado.Id });
                ModelState.AddModelError("Imp", Textos.ErrorImpresionTarjetaDeAcceso);
            }
        }

        private void SetearVista(DatosUsuario datosUsuario)
        {
            var puestoDeTrabajo = servicio.ListarPuestosDeTrabajoPorNombrePc(datosUsuario.NombrePc, datosUsuario.CentroId).Where(x => x.PidePatente).FirstOrDefault();
            if (puestoDeTrabajo != null && puestoDeTrabajo.VideoCamaras != null && puestoDeTrabajo.VideoCamaras.Any())
            {
                var camaraPatente = puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).First();
                var camaraCp = puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).Last();
                ViewBag.CodigoCamaraPatente = camaraPatente.Codigo;
                ViewBag.CodigoCamaraPatenteDir = camaraPatente.Directorio;
                ViewBag.CodigoCamaraCP = camaraCp.Codigo;
                ViewBag.CodigoCamaraCPDir = camaraCp.Directorio;
            }
            else
            {
                ViewBag.CodigoCamaraPatente = "";
                ViewBag.CodigoCamaraPatenteDir = "";
                ViewBag.CodigoCamaraCP = "";
                ViewBag.CodigoCamaraCPDir = "";
            }
            ViewBag.Materiales = servicio.ListarMaterialesPorWorkflow(225, datosUsuario.CentroId).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            ViewBag.PuestoDeTrabajo = puestoDeTrabajo != null ? puestoDeTrabajo.ToJson() : null;
            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.AvanzaAutomatico = servicio.AvanzaCpe(datosUsuario.CentroId);
        }

        private byte[] DibujarEtiqueta(byte[] foto, CargaDeCupoDto model, int fontSize = 25)
        {
            var etiqueta = $"Ingreso: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} Tarjeta: {model.Numero} CP: {model.NumeroCartaPorte}";
            var etiquetad = new Dictionary<string, string>
            {
                {"Ingreso: ", $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}"},
                {"Tarjeta: ", $"{model.Numero}"},
                {"CP: ", $"{model.NumeroCartaPorte}"}
            };
            Bitmap imagenCP;
            using (var ms = new MemoryStream(foto))
            {
                imagenCP = new Bitmap(ms);
            }
            PointF posicionEtiqueta = new PointF(0, 0);

            using (Graphics graphics = Graphics.FromImage(imagenCP))
            {
                using (Font arialFontb = new Font("Arial", fontSize, FontStyle.Bold))
                {
                    using (Font arialFont = new Font("Arial", fontSize))
                    {
                        var sizeEtiqueta = graphics.MeasureString(etiqueta, arialFont);
                        var rect = new RectangleF(posicionEtiqueta.X, posicionEtiqueta.Y, sizeEtiqueta.Width, sizeEtiqueta.Height);
                        graphics.FillRectangle(Brushes.White, rect);

                        foreach (var k in etiquetad.Keys)
                        {
                            graphics.DrawString(k, arialFont, Brushes.Black, posicionEtiqueta);
                            posicionEtiqueta.X += graphics.MeasureString(k, arialFont).Width;
                            graphics.DrawString(etiquetad[k], arialFontb, Brushes.Black, posicionEtiqueta);
                            posicionEtiqueta.X += graphics.MeasureString(etiquetad[k], arialFontb).Width;
                        }
                    }
                }
            }
            var resultado = ImageToByte(imagenCP);
            imagenCP.Dispose();
            return resultado;
        }

        private VideoCamaraDto ObtenerVideoCamaraPorPuesto(string puestodetrabajoid, bool fotoPatente)
        {
            if (int.TryParse(puestodetrabajoid, out int n))
            {
                var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajo(int.Parse(puestodetrabajoid));

                if (puestoDeTrabajo.VideoCamaras != null && puestoDeTrabajo.VideoCamaras.Any())
                {
                    return fotoPatente ? puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).First() : puestoDeTrabajo.VideoCamaras.OrderBy(x => x.Id).Last();
                }
                else
                {
                    throw new Exception("No hay videocamaras asociadas al puesto de trabajo");
                }
            }
            else
            {
                throw new Exception("No se pudo detectar correctamente el puesto de trabajo");
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 70L);
            using (var stream = new MemoryStream())
            {
                img.Save(stream, jgpEncoder, myEncoderParameters);
                return stream.ToArray();
            }
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarCargaDeCupo { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void MostrarPorCartel(string nombrePc, string mensaje, int centroId, string patente)
        {
            var puestoDeTrabajo = servicio.ObtenerPuestoDeTrabajoPorNombrePc(nombrePc, centroId);
            var mensajesCartel = servicio.ObtenerMensajesCartelLed(CodigoMensajeCartelLed.GaritaIngresoAsignarCalle);

            try
            {
                var mensajes = mensajesCartel.Select(s =>
                    new EnviarMensajeCarteLed
                    {
                        Mensaje = string.Format(s.Mensaje, mensaje, patente),
                        PuestoDeTrabajoId = puestoDeTrabajo.Id,
                        NumeroPrograma = s.Programa,
                        NumeroTrama = s.Trama,
                        NumeroVariable = s.Variable,
                        SegundosDeEspera = s.SegundosDeEspera
                    }
                    ).ToList();

                servicioComandos.Ejecutar(new EnviarMensajesAsincronoCartelLed { Mensajes = mensajes });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        [DatosUsuario]
        public ActionResult ObtenerMaterial(bool esGrano, DatosUsuario datosUsuario)
        {
            var materiales = servicio.ListarMaterialGranoPorCentro(datosUsuario.CentroId, esGrano).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);

            return Json(materiales, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerDatosDeSap(string numero, DatosUsuario datosUsuario)
        {
            var consultaOrdenDeCarga = new ConsultaOrdenDeCarga
            {
                Centro = servicio.ObtenerCentro(datosUsuario.CentroId).CodigoSAP,
                Patente = numero.ToUpper()
            };
            var datosRequest = new ConsultaOrdenDeCargaRequest
            {
                ConsultaOrdenDeCarga = consultaOrdenDeCarga
            };

            try
            {
                var respuestaConsultaOrdenCarga = servicioSap.ConsultaOrdenDeCarga(datosRequest);
                var datosSap = new List<OrdenCargaFasDto>();

                if (respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Any())
                {
                    var ordenCargaFas = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida;
                    int count = respuestaConsultaOrdenCarga.ConsultaOrdenDeCargaResponse.Salida.Count();

                    for (int i = 0; i < count; i++)
                    {
                        var material = servicio.ObtenerMaterialPorCodigoSap(ordenCargaFas[i].MATNR.TrimStart(new[] { '0' }));

                        if (material == null)
                        {
                            return Json(new { datosSap = -1, error = string.Format(Textos.OrdenCargaFAS_MaterialInexistente, ordenCargaFas[i].MATNR) }, JsonRequestBehavior.AllowGet);
                        }

                        var itemSap = new OrdenCargaFasDto
                        {
                            MaterialId = material.Id,
                            MaterialDesc = material.Descripcion,
                        };
                        datosSap.Add(itemSap);
                    }
                    return Json(new { datosSap }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { datosSap = -1, error = Textos.OrdenCargaFas_Error }, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerCPE(DatosUsuario datosUsuario, long numeroCtg, string tarjeta = "", bool esEpecial = false)
        {
            try
            {
                var estadoErroresBloqueantes = new List<string> { "AN", "RE" };
                var estadoPermiteIngresar = new List<string> { "AC", "CF", "CO" };
                log.Debug("Obteniendo CTG {0} en carga de Cupo.", numeroCtg);
                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = numeroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId, ConsultaMinima = true }) as ResultadoCartaPorteElectronica;
                log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener carta de porte CTG-CPE en carga de Cupo. {0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo carta de porte en carga de Cupo. CTG-CPE {0}", numeroCtg);
                var errorCode = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "3";
                var errorMsg = cartaPorteResponse.Errores.Values.FirstOrDefault();
                var pdfString = string.Empty;
                var pdfSustentableString = string.Empty;

                if (errorCode != "2")
                {
                    if (!cartaPorteResponse.HayErrores && !estadoPermiteIngresar.Contains(cartaPorteResponse.Cpe.EstadoCpe))
                    {
                        if (estadoErroresBloqueantes.Any(a => a == cartaPorteResponse.Cpe?.EstadoCpe?.ToUpper()?.Trim()))
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

                    if (cartaPorteResponse.PdfImage != null)
                    {
                        cartaPorteResponse.PdfImage = DibujarEtiqueta(cartaPorteResponse.PdfImage, new CargaDeCupoDto()
                        {
                            Numero = tarjeta,
                            NumeroCartaPorte = numeroCtg.ToString()
                        }, 18);
                        pdfString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(cartaPorteResponse.PdfImage));

                        if (esEpecial)
                        {
                            cartaPorteResponse.PdfImageSustentable = DibujarSelloSustentable(cartaPorteResponse.PdfImage);
                            if (cartaPorteResponse.PdfImageSustentable != null)
                            {
                                pdfSustentableString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(cartaPorteResponse.PdfImageSustentable));
                            }
                        }
                    }
                    else
                    {
                        if (!estadoErroresBloqueantes.Any(a => a == cartaPorteResponse.Cpe?.EstadoCpe))
                        {
                            var cartaPorteImagen = servicioComandos.Ejecutar(new ConsultarImagenCpe { NroCtg = numeroCtg }) as ResultadoConsultarImagenCpe;
                            if (cartaPorteImagen.HayErrores)
                            {
                                errorMsg = "No se pudo obtener la imagen de la CP desde Afip, por favor tomarlo manualmente.";
                                errorCode = "4";
                            }
                            else
                            {
                                cartaPorteResponse.PdfImage = DibujarEtiqueta(cartaPorteImagen.PdfImage, new CargaDeCupoDto()
                                {
                                    Numero = tarjeta,
                                    NumeroCartaPorte = numeroCtg.ToString()
                                }, 18);
                                pdfString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(cartaPorteResponse.PdfImage));

                                if (esEpecial)
                                {
                                    cartaPorteResponse.PdfImageSustentable = DibujarSelloSustentable(cartaPorteResponse.PdfImage);
                                    if (cartaPorteResponse.PdfImageSustentable != null)
                                    {
                                        pdfSustentableString = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(cartaPorteResponse.PdfImageSustentable));
                                    }
                                }
                            }
                        }
                    }
                }

                return new JsonResult()
                {
                    Data = new { cartaPorteResponse.Cpe, CodigoDeError = errorCode, Error = errorMsg, PdfImageBase64 = pdfString, PdfImageSustentableBase64 = pdfSustentableString },
                    ContentType = "application/json",
                    ContentEncoding = System.Text.Encoding.UTF8,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte CTG-CPE en carga de Cupo. {0}", numeroCtg);
                throw;
            }
        }

        private void AperturaDeBarrera(string codigo)
        {
            try
            {
                servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera { CodigoDispositivo = codigo });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo levantar la barrera");
            }
        }

        private byte[] DibujarSelloSustentable(byte[] PdfImage)
        {
            if (PdfImage != null)
            {
                var resultado = servicioComandos.Ejecutar(new AgregarMarcaSustentable
                {
                    SoloDibujar = true,
                    PdfImage = PdfImage
                }) as ResultadoCartaPorteElectronica;
                return resultado.PdfImageSustentable;
            }
            return null;
        }

        private void CargarCartaPorte(int id, DatosUsuario datosUsuario)
        {
            var cargaDeCupo = servicio.ObtenerCupoPorId(id);
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapMolinosAgro = firma.ObtenerFirmaSinLogo().CodigoSAP;
            var workflow = "";
            var tipoComercialId = 0;
            if ((cargaDeCupo.TitularCartaPorteCodigoSap == codigoSapMRP && (cargaDeCupo.RtteComercialCodigoSap == null || cargaDeCupo.RtteComercialCodigoSap == codigoSapMRP || cargaDeCupo.RtteComercialCodigoSap == codigoSapMolinosAgro)) ||
                ((cargaDeCupo.TitularCartaPorteCodigoSap == codigoSapMolinosAgro) && (cargaDeCupo.RtteComercialCodigoSap == null || (cargaDeCupo.RtteComercialCodigoSap == codigoSapMolinosAgro))))
            {
                workflow = ConfigurationManager.AppSettings["workflowRedespacho"];
                tipoComercialId = 8;
            }
            else if (!string.IsNullOrEmpty(cargaDeCupo.TitularCartaPorteCodigoSap))
            {
                workflow = ConfigurationManager.AppSettings["WorkflowIngresoPorCompra"];
                tipoComercialId = 4;
            }
            if (cargaDeCupo == null || string.IsNullOrEmpty(workflow)) { 
                ModelState.AddModelError("avanceCpe", "No hay Carga De Cupo");
                return; }
            var puesto = servicio.ObtenerPuestoDeTrabajo(cargaDeCupo.PuestoDeTrabajoId);
            var orden = servicioComandos.Ejecutar(new ConsultarCPDigital { CentroId = datosUsuario.CentroId, NroCtg = long.Parse(cargaDeCupo.CTG), Usuario = datosUsuario.NombreUsuario }) as ResultadoCartaPorteElectronica;
            if (orden != null && orden.Cpe != null)
            {
                orden.Cpe.TipoComercialId = tipoComercialId;
                orden.Cpe.Id = 0;
                orden.Cpe.CEE = "99";
                if (orden.Cpe.Vehiculos != null) orden.Cpe.Vehiculos.FirstOrDefault().Primero = true;
                try
                {
                    CargarAutomaticaCartaPorte(workflow, puesto.NombrePuesto, "", "", orden.Cpe, datosUsuario);
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }
            }
        }

        private void CargarAutomaticaCartaPorte(string workflow, string puestoDeTrabajo, string fotoMesaDigitalizacion1, string fotoMesaDigitalizacion2, CartaPorteDto orden, DatosUsuario datosUsuario)
        {
            log.Debug("Iniciando Carga de Carta de Porte número {0}", orden.NroCartaPorte);
            var workflowObj = servicio.ObtenerWorkflowPorCodigo(workflow);
            var vehiculos = orden.Vehiculos;
            ViewBag.aceptaPendiente = true;

            if (orden.Cpe && workflowObj.TipoDeWorkflow == TipoDeWorkflow.Egreso)
            {
                if (string.IsNullOrEmpty(orden.NroCartaPorte))
                {
                    var sequenciaNroCartaPorteCPE = servicio.ObtenerSequenciaNumeroCTGCartaPorteElectronica();
                    orden.NroCartaPorte = $"{DateTime.Now.ToString("yyyyMMdd")}{sequenciaNroCartaPorteCPE.ToString("D4")}";
                }
            }
            if (datosUsuario.CentroId == 0)
            {
                log.Debug("El usuario {0} no tiene seleccionado un centro", datosUsuario.NombreUsuario);
                ModelState.AddModelError("avanceCpe", $"El usuario {datosUsuario.NombreUsuario} no tiene seleccionado un centro");
                return;
            }
            if (!Validar(orden, datosUsuario))
            {
                log.Debug("No Válido");
                ModelState.AddModelError("avanceCpe", $"No Válido");

                return;
            }
            if (ModelState.IsValid && vehiculos != null && vehiculos.Count() != 0)
            {
                var response = servicio.NumeroCartaPorteValido(orden.NroCartaPorte, datosUsuario.CentroId, workflowObj.Descripcion, orden.Cpe);
                if (!response.Valida)
                {
                    log.Debug("No se puede crear la CP {0}. Detalle: {1}", orden.NroCartaPorte, response.Error);
                    ModelState.AddModelError("avanceCpe", $"No se puede crear la CP {orden.NroCartaPorte}. Detalle: {response.Error}");

                    return;
                    //return View(orden);
                }

                if (vehiculos.Count() != vehiculos.GroupBy(x => x.Patente).Count())
                {
                    log.Debug("No se puede crear la CP {0}. Alguna de las patentes está duplicada");
                    ModelState.AddModelError("avanceCpe", $"No se puede crear la CP. Alguna de las patentes está duplicada");

                    return;

                    //return View(orden);
                }

                if (vehiculos.Any(vehiculo => workflows.ObtenerWorkflowPorPatente(vehiculo.Patente) != null))
                {
                    log.Debug("No se puede crear la CP. Alguna de las patentes esta ingresada en un workflow en ejecución");
                    ModelState.AddModelError("avanceCpe", "No se puede crear la CP. Alguna de las patentes esta ingresada en un workflow en ejecución");
                    return;

                    //return View(orden);
                }

                var tipoComercial = servicio.ObtenerTipoComercial(orden.TipoComercialId);

                if (tipoComercial?.PesoMaximoDocumentoIngreso != null && tipoComercial?.PesoMaximoDocumentoIngreso != 0)
                {
                    if (vehiculos.Any(vehiculo => vehiculo.PesoBrutoOrigen > tipoComercial.PesoMaximoDocumentoIngreso))
                    {
                        log.Debug("El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                    ModelState.AddModelError("avanceCpe", "El Tipo comercial tiene configurado un peso maximo en ingreso y fue excedido");
                        return;

                        //return View(orden);
                    }
                }
                var tipoVehiculo = ObtenerTipoVehiculoPorPatente(vehiculos.FirstOrDefault().Patente, vehiculos.FirstOrDefault().PatenteAcoplado, workflow, datosUsuario, vehiculos.FirstOrDefault().PatenteAcoplado2);
                if(tipoVehiculo != null && tipoVehiculo.HayErrores)
                {
                    log.Debug("Fallo validacion tipo vehiculo");
                    ModelState.AddModelError("avanceCpe", "Fallo validacion tipo vehiculo");
                    return;
                }
                orden.TipoVehiculo = tipoVehiculo.Categoria.HasValue ? tipoVehiculo.Categoria.Value : TipoVehiculo.Camión;
                var resultadoChofer = SetearChofer(orden.Chofer);
                if (resultadoChofer == false)
                {
                    log.Debug("No se pudo dar de alta o asociar el chofer a la CP");
                    ModelState.AddModelError("avanceCpe", "No se pudo dar de alta o asociar el chofer a la CP");
                    return;
                }

                var transportistaId = orden.TransportistaId ?? 0;
                var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
                orden.TransportistaId = transportistaId;
                if (!resultadoTransportista)
                {
                    log.Debug("No se pudo dar de alta o asociar el transportista a la CP");
                    ModelState.AddModelError("avanceCpe", "No se pudo dar de alta o asociar el transportista a la CP");
                    return;
                }

                if (!ValidarCupo(orden, datosUsuario, workflowObj.TipoDeWorkflow == TipoDeWorkflow.Ingreso))
                {
                    return;
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
                var cupo = servicio.ObtenerCupoPorCupoSap(orden.Cupo); // TODO Optimizar consulta del cupo para determinar si es especial

                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                var instanceIds = new List<Guid>();

                log.Info("CargarCartaPorte: Iniciando carga de workflow/s para los/el vehiculo/s: " + orden.VehiculoJson);
                foreach (var vehiculo in vehiculos)
                {
                    var controlRecorrido = new ControlRecorridoDto
                    {
                        Actividad = Textos.ActCargarCartaPorte,
                        ActividadXaml = "CargarCartaPorte",
                        PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                        NombreUsuario = datosUsuario.NombreUsuario
                    };
                    var resultadoActividad = servicioWf.CargarCartaPorte(orden, vehiculo, datosUsuario.CentroId, workflow, workflowDefinicionId, datosUsuario.NombreUsuario, controlRecorrido) as ResultadoCrearWorkflow;
                    if (resultadoActividad.HayErrores)
                    {
                        log.Debug(resultadoActividad.Errores.FirstOrDefault().Value);
                        ModelState.AddModelError("avanceCpe", resultadoActividad.Errores.FirstOrDefault().Value);
                        return;
                    }
                    orden.Id = resultadoActividad.Id;
                    instanceIds.Add(resultadoActividad.InstanciaWorkflowId);
                }

               
            }
          
        }

        protected virtual bool Validar(CartaPorteDto orden, DatosUsuario usuario)
        {
            var codigoSapMolinosAgro = firma.ObtenerFirmaSinLogo().CodigoSAP;
            var codigoSapMRP = ConfigurationManager.AppSettings["CodigoSapMRP"];
            var codigoSapTitular = servicio.ObtenerProveedor(orden.TitularCartaPorteId).CodigoSap;
            var codigoDeEstablecimiento = orden.CodEstab;
            var remitente = servicio.ObtenerProveedor(orden.RtteComercialId);
            var otroRecorridoDelChofer = orden.Chofer != null ? servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id) : null;

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

        private bool ValidarCupo(CartaPorteDto orden, DatosUsuario usuario, bool esIngreso)
        {
            //No validamos Si no requiere cupo o si el destino no es un centro de MOA
            if (!orden.RequiereCupo || !orden.ValidarCupo || (!esIngreso && orden.EsClienteDestinatario) || (!esIngreso && !orden.Cupo.StartsWith("MOL")))
            {
                return true;
            }
            var resultado = servicio.ValidarCupo(orden.Cupo, usuario.CentroId, orden.NroCartaPorte);
           
            if (resultado.Valido && !resultado.YaAsignado)
            {
                return true;
            }

            if (resultado.YaAsignado)
            {
                log.Debug(resultado.MensajeError);
                ModelState.AddModelError("avanceCpe", "Cupo ya asignado");

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
                        ModelState.AddModelError("avanceCpe", string.Format(Textos.Material_CodigoSAPNoExiste, respuesta.MATERIAL));
                        return false;
                    }
                    if (esEspecial)
                    {
                        ModelState.AddModelError("avanceCpe", "Cupo sustentable");
                        return false;
                    }
                    return true;
                }
                log.Debug("ValidarCupoEnSap Respuesta {0} no encontrado", orden.Cupo);

                ModelState.AddModelError("avanceCpe", $"ValidarCupoEnSap Respuesta {orden.Cupo} no encontrado");
                return false;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al validar cupo en SAP: ");

                ModelState.AddModelError("avanceCpe", $"Error al validar cupo en SAP");
                return false;
            }
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

        protected bool SetearChofer(ChoferDto choferDto)
        {
            if (!ModelState.IsValid|| choferDto == null)
            {
                return false;
            }            
            log.Info("SetearChofer para el chofer con el CUIL: " + choferDto.Cuil);
            var chofer = servicio.BuscarChoferes(new ChoferFiltro { Cuil = choferDto.Cuil }).FirstOrDefault();
            if (chofer != null) //Chofer existente
            {
                log.Info("SetearChofer se actualizará el chofer con CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new ModificarChofer() { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
            }
            else //ChoferNuevo
            {
                log.Info("SetearChofer se dará de alta el chofer con el CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new CrearChofer { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
                choferDto.Id = (resultadoChofer as ResultadoCrear).Id;
            }
            return true;
        }

        protected bool SetearTransportista(ref int transportistaId, int tipoComercialId, bool esTransportista)
        {
            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            if (tipoComercial.TransportistaEsProveedor && (transportistaId == 0))
            {
                log.Debug("El transportista es obligatorio para el tipo comercial");
                ModelState.AddModelError("avanceCpe", string.Format(Textos.Error_Requerido, Textos.Transportista));
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
                    ModelState.AddModelError("avanceCpe", string.Format(Textos.Error_ProveedorInvalido));
                    transportistaId = 0;
                    return false;
                }

                var transportista = servicio.ObtenerTransportistaPorCuit(proveedor.Cuil);
                if (transportista != null) //Transportista Existente
                {
                    transportistaId = transportista.Id;
                }
                else //Creo el nuevo transportista
                {
                    try
                    {
                        var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                        {
                            Dto = new TransportistaDto
                            {
                                Cuit = proveedor.Cuil,
                                Domicilio = proveedor.Domicilio,
                                LocalidadId = proveedor.LocalidadId,
                                ProvinciaId = proveedor.ProvinciaId,
                                RazonSocial = proveedor.RazonSocial
                            }
                        });
                        if (resultadoTransportista.HayErrores)
                        {
                            resultadoTransportista.Errores.ToList()
                                                  .ForEach(f => ModelState.AddModelError("avanceCpe", f.Value));
                            ModelState.AgregarErrores(resultadoTransportista);
                            return false;
                        }
                        transportistaId = (resultadoTransportista as ResultadoCrear).Id;
                    }
                    catch
                    {
                        ModelState.AddModelError("avanceCpe", string.Format(Textos.Error_ProveedorInvalido));
                    }
                }
            }
            return true;
        }

        public ResultadoEscalables ObtenerTipoVehiculoPorPatente(string patente, string acoplado, string workflow, DatosUsuario datosUsuario, string acoplado2 = "")
        {
            try
            {
                log.Debug("Obteniendo Tipo de vehiculo por patente {0} workflow {1}", patente, workflow);
                return servicioComandos.Ejecutar(new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = acoplado2, Usuario = datosUsuario.NombreUsuario }) as ResultadoEscalables;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el tipo de vehiculo por patente {0}", patente);
                throw;
            }
        }
    }
}
