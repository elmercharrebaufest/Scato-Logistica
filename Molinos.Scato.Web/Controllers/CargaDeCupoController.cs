using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

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

        public CargaDeCupoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows, ZSDWS_SCATO servicioSap, IServicioOrquestador servicioOrquestador, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
            this.workflows = workflows;
            this.servicioSap = servicioSap;
            this.servicioOrquestador = servicioOrquestador;
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
        public ActionResult Index(CargaDeCupoDto model, string imagenCartaPorte, DatosUsuario datosUsuario)
        {
            if (model.CircuitoNoGranos)
            {
                return RedirectToAction("IndexNoGranos", model);
            }
            model.ImagenCartaPorte = imagenCartaPorte.Replace("data:image/jpg;base64,", "");
            ViewBag.Materiales = servicio.ListarMaterialesPorWorkflow(225, datosUsuario.CentroId).ToSelectList(f => f.MaterialId.ToString(), f => f.MaterialDesc);
            ModelState.Remove("MaterialId");
            ModelState.Remove("Especial");
            if (model.CPE)
            {
                ModelState.Remove("NumeroCartaPorte");
            } else
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
                    if (!model.NoAsignaCalleEnGaritaEntrada)
                    {
                        log.Debug("Asignar Calle: Resultado Id= {0}, Patente: {1}, MaterialId: {2}", resultado.Id, model.Patente, model.MaterialId);
                        var turnoActivo = InformarArribo(model.CPE ? model.CTG : model.NumeroCartaPorte, datosUsuario.CentroId, model.Patente, model.MaterialId);
                        AsignarCalle(resultado.Id, turnoActivo, model.CPE ? model.CTG : model.NumeroCartaPorte, datosUsuario.CentroId, datosUsuario.NombrePc, model.Patente);

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
            ModelState.Remove("NumeroCartaPorte");
            ModelState.Remove("CTG");
            if (ModelState.IsValid)
            {
                log.Debug("Asignación de Cupo No Granos {0}, tarjeta {1}, centro {2}, CP {3}", model.Cupo, model.Numero, datosUsuario.CentroId);
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
                    if (!model.NoAsignaCalleEnGaritaEntrada && model.MaterialId != 0)
                    {
                        log.Debug("Asignar Calle: Resultado Id= {0}, Patente: {1}, MaterialId: {2}", resultado.Id, model.Patente, model.MaterialId);
                        var turnoActivo = InformarArribo(model.NumeroCartaPorte, datosUsuario.CentroId, model.Patente, model.MaterialId);
                        AsignarCalle(resultado.Id, turnoActivo, model.NumeroCartaPorte, datosUsuario.CentroId, datosUsuario.NombrePc, model.Patente, true);
                        model.MaterialId = 0;
                    }
                    if(!model.NoAsignaCalleEnGaritaEntrada && model.MaterialId == 0 && ModelState.IsValid)
                    {
                        MostrarPorCartel(datosUsuario.NombrePc,"Mesa FAS", datosUsuario.CentroId, model.Patente);
                        ViewBag.EsCircuitoNoGranosSinMaterial = true;
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

                        if(!circuitoNoGranos)
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
        public JsonResult ValidarCupoEnSap(string cupo, DatosUsuario datosUsuario)
        {
            var model = new CargaDeCupoDto();

            ////model.MaterialId = 4;
            ////model.RespuestaSap = "hola";
            ////model.ProveedorDescripcion = "SUCESION DE RUEDA ALFREDO EDUARDO";
            ////model.ProveedorCuit = "20-00200069-6";
            ////model.MaterialDescripcion = "Semilla de Soja";
            ////model.FechaSap = "2019-01-29";
            ////model.Especial = false;
            ////model.Camara = "Fabrica";
            ////return Json(new { model }, JsonRequestBehavior.AllowGet);

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
                        return Json(new { model }, JsonRequestBehavior.AllowGet);
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
                }
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
            catch (Exception ex) {
                return Json(new { datosSap = -1, error = Textos.OrdenCargaFas_Error }, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public JsonResult ObtenerCPE(DatosUsuario datosUsuario, long numeroCtg, string tarjeta = "")
        {
            try
            {
                var estadoErroresBloqueantes = new List<string> { "AN", "RE" };
                log.Debug("Obteniendo CTG {0} en carga de Cupo.", numeroCtg);
                var cartaPorteResponse = servicioComandos.Ejecutar(new ConsultarCPDigital { NroCtg = numeroCtg, Usuario = datosUsuario.NombreUsuario, CentroId = datosUsuario.CentroId, ConsultaMinima = true }) as ResultadoCartaPorteElectronica;
                log.Debug(cartaPorteResponse.HayErrores ? "Error al obtener carta de porte CTG-CPE en carga de Cupo. {0}: " + cartaPorteResponse.Errores.Values.First() : "Devolviendo carta de porte en carga de Cupo. CTG-CPE {0}", numeroCtg);
                var errorCode = cartaPorteResponse.HayErrores ? cartaPorteResponse.Errores.Keys.First() : "3";
                var errorMsg = cartaPorteResponse.Errores.Values.FirstOrDefault();
                var pdfString = string.Empty;

                if (errorCode != "2")
                {
                    if (!cartaPorteResponse.HayErrores && cartaPorteResponse.Cpe?.EstadoCpe != "AC")
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
                            }
                        }
                    }
                }

                return new JsonResult()
                {
                    Data = new { cartaPorteResponse.Cpe, CodigoDeError = errorCode, Error = errorMsg, PdfImageBase64 = pdfString},
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

    }
}
