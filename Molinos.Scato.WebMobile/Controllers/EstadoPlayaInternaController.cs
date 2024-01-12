using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.EstadoDeCallePlayaInterna)]
    public class EstadoPlayaInternaController : ConsultasController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;
        private List<TipoCalle> listaCalles;
        private List<TipoCalle> callesLlamadoManual;

        public EstadoPlayaInternaController(
            ILogger log,
            IServicioRepositorio servicio,
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos

            ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioComandos = servicioComandos;
            this.listaCalles = new List<TipoCalle> {
                TipoCalle.PlayaInterna,
                TipoCalle.PlantaNoGranos,
                TipoCalle.EnTransito,
                TipoCalle.SalidaNoGranos,
                TipoCalle.EsperaAduanaNoGranos,
                TipoCalle.EnTransitoGranos,
                TipoCalle.PreBalanzaGranos,
                TipoCalle.SalidaGranos
            };

            this.callesLlamadoManual = new List<TipoCalle> {
                TipoCalle.PreBalanzaGranos
            };
        }

        public ActionResult Index()
        {
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            ViewBag.MaterialesGranos = servicio.ListarMaterialGranoPorCentro(5, true).Where(x => x.MostrarEnWebMobile).ToList();
            return View(tipoCallePlantaLista);
        }

        public JsonResult ObtenerCalles(string tiposCalleStr)
        {
            if (string.IsNullOrEmpty(tiposCalleStr))
                return Json(new List<TipoCallePlantaDto>(), JsonRequestBehavior.AllowGet);

            var tiposCalle = new List<TipoCalle>();
            var arrTipoCallesStr = tiposCalleStr.Split(',');
            foreach (var tipoCalleStr in arrTipoCallesStr)
            {
                var tipoCalleInt = int.Parse(tipoCalleStr);
                tiposCalle.Add((TipoCalle)tipoCalleInt);
            }
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            var noeditables = servicio.ListarIdCallesNoEditables();
            foreach (var item in tipoCallePlantaLista)
            {
                foreach (var calle in item.Calles)
                {
                    if (noeditables.Contains(calle.CalleId))
                        calle.AutomatismoActivo = true;
                }
            }
            return Json(tipoCallePlantaLista.Where(x => tiposCalle.Contains(x.TipoCalle)).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarDetalleCamion(string patente, int calleId)
        {
            var model = servicio.ObtenerInfoPatente(patente, calleId);
            if (model.RecorridoId != null)
            {
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                model.CorrespondeConfirmarCargaDescarga = servicio.ExisteConfirmacionCargaDescargaDeRecorrido(model.RecorridoId ?? 0) && servicio.TienePermiso(usuario.Value, PermisosScato.ConfirmacionCargaDescarga);
            }
            return PartialView("_DetalleCamion", model);
        }

        [HttpPost]
        public ActionResult ConfirmarCargaDescarga(Guid workflowInstance, int recorridoId)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var usuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier);
                var confirmacion = new ConfirmacionCargaDescargaDto()
                {
                    RecorridoId = recorridoId,
                    FechaConfirmacion = DateTime.Now,
                    Confirmado = true,
                    PendienteConfirmacion = false,
                    NombreUsuario = usuario.Value
                };
                var resultado = servicioComandos.Ejecutar(new ActualizarConfirmacionCargaDescarga
                {
                    Dto = confirmacion
                });

                if (!resultado.HayErrores)
                {
                    var controlRecorrido = new ControlRecorridoDto()
                    {
                        WorkflowInstanceId = workflowInstance,
                        Actividad = EtapaWorkflow.ConfirmacionCargaDescarga,
                        ActividadXaml = EtapaWorkflow.ConfirmacionCargaDescarga,
                        NombreUsuario = usuario.Value
                    };
                    resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = controlRecorrido
                    });
                }
                AgregarErroresARespuesta(resultado, response);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al confirmar Carga/Descarga para el workflow {workflowInstance}";
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfiguracionLlamadoAutomaticoPreBalanza(bool activar)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var configuracionLlamadoAutomatico = servicio.ObtenerConfiguracionGeneral(ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, ConfiguracionGeneral.PreBalanza.LlamadoAutomatico);
                if (configuracionLlamadoAutomatico == null)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "No existe la configuración de Llamado Automatico de Prebalanza", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                configuracionLlamadoAutomatico.Valor = activar.ToString();
                var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionGeneral
                {
                    Dto = configuracionLlamadoAutomatico
                });
                AgregarErroresARespuesta(resultado, response);
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error al Activar o Desactivar el Llamado Automatico de PreBalanza");
                var errorMessage = $"Error al Activar o Desactivar el Llamado Automatico de PreBalanza";
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LlamarCallePrebalanza(int callePrebalanzaId)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var callePreBalanza = servicio.ObtenerCalle(callePrebalanzaId);
                if (callePreBalanza.FechaLLamada.HasValue)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La {callePreBalanza.Nombre} ya ha sido llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var camionesEnPrebalanza = servicio.ListarCallePorRecorridoPorCalleId(callePrebalanzaId);
                if (!camionesEnPrebalanza.Any())
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No hay camiones en la {callePreBalanza.Nombre}.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var primerCamionEnPrebalanza = camionesEnPrebalanza.OrderBy(x => x.FechaIngeso).FirstOrDefault();
                if (primerCamionEnPrebalanza?.CalleRecorridoId == null && primerCamionEnPrebalanza != null)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"El camion {primerCamionEnPrebalanza.Patente} no tiene una calle de Playa Interna asignada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var callePlayaInterna = servicio.ObtenerCalle(primerCamionEnPrebalanza.CalleRecorridoId.Value);
                if (!ExisteSlotsDisponibles(callePlayaInterna, camionesEnPrebalanza.Count()))
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La {callePlayaInterna.Nombre} no tiene espacios suficientes.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                servicioComandos.Ejecutar(new CrearCallePreBalanzaPlayaInterna
                {
                    CallePlayaInternaId = callePlayaInterna.Id,
                    CallePreBalanzaId = callePrebalanzaId
                });

                var resultadoInsertarCartelLed = servicioComandos.Ejecutar(new InsertarSlotMensajeCartelLed()
                {
                    Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                    CalleId = callePrebalanzaId
                }) as ResultadoMensajeCartelLed;

                EnviarMensajeLlamadoACartelPrebalanza(resultadoInsertarCartelLed);
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo llamar la fila Prebalanza con Id {callePrebalanzaId}");
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"Ocurrió un error al llamar la fila.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LiberarCallePrebalanza(int callePrebalanzaId)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                
                var callePreBalanza = servicio.ObtenerCalle(callePrebalanzaId);
                if (!callePreBalanza.FechaLLamada.HasValue || !callePreBalanza.Bloqueada)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La {callePreBalanza.Nombre} no está siendo llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var callePlayaInterna = servicio.ObtenerCallePrebalanzaPlayaInterna(callePrebalanzaId);
                if (callePlayaInterna == null)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La {callePreBalanza.Nombre} no está siendo llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                servicioComandos.Ejecutar(new EliminarCallePreBalanzaPlayaInterna()
                {
                    CallePlayaInternaId = callePlayaInterna.CallePlayaInterna.Id,
                    CallePreBalanzaId = callePrebalanzaId
                });

                var resultadoLimpiarCartelLed = servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
                {
                    Codigo = CodigoMensajeCartelLed.LlamadoCallePreBalanza,
                    CalleId = callePrebalanzaId
                }) as ResultadoMensajeCartelLedReordenado;

                LimpiarMensajeLlamadoACartelPrebalanza(resultadoLimpiarCartelLed);
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo liberar la fila Prebalanza con Id {callePrebalanzaId}");
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = "Ocurrió un error al liberar la fila.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MostrarDetalleCalle(int calleId)
        {
            var model = servicio.ObtenerInfoCalle(calleId);

            return PartialView("_DetalleCalle", model);
        }

        private List<TipoCallePlantaDto> ObtenerTiposDeCallesPlanta()
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");

            var centroId = int.Parse(centro.Value);
            var camiones = servicio.ObtenerEstadoDeCalle();
            camiones.ForEach(camion =>
            {
                if (camion.EsSojaIMPO)
                {
                    camion.ColorFondo = ValoresPorDefecto.ColorFondoSojaIMPO;
                    camion.ColorTexto = ValoresPorDefecto.ColorTextoSojaIMPO;
                }
            });
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle) && !x.Deshabilitada).OrderBy(x => x.Posicion).ToList();
            var tipoCallePlantaLista = new List<TipoCallePlantaDto>();

            foreach (var calle in calles)
            {
                var tipoCallePlanta = tipoCallePlantaLista.FirstOrDefault(q => q.TipoCalle == calle.TipoCalle);
                if (tipoCallePlanta == null)
                {
                    tipoCallePlanta = new TipoCallePlantaDto
                    {
                        TipoCalle = calle.TipoCalle
                    };
                    tipoCallePlantaLista.Add(tipoCallePlanta);
                }

                var callePlanta = new CallePlantaDto
                {
                    CalleId = calle.Id,
                    CalleDesc = calle.Nombre,
                    ColorFondo = calle.ColorFondo ?? "#000",
                    ColorTexto = calle.ColorTexto ?? "#fff",
                    LimiteDeCamiones = calle.CantidadDeCamiones,
                    Bloqueada = calle.Bloqueada,
                    EsPrimero = false,
                    EsUltimo = false,
                    MaterialId = calle.MaterialId,
                    LlamadoManual = callesLlamadoManual.Any(x => x == calle.TipoCalle)
                };
                tipoCallePlanta.Calles.Add(callePlanta);

                foreach (var camion in camiones.Where(q => q.CalleId == calle.Id))
                {
                    var camionPlanta = new CamionPlantaDto
                    {
                        CamionId = camion.Id,
                        Patente = camion.Patente,
                        Escalable = camion.Escalable,
                        UltimoDeLaFila = camion.UltimoDeLaFila,
                        ColorFondo = camion.ColorFondo ?? "#000",
                        ColorTexto = camion.ColorTexto ?? "#fff",
                        CalleId = camion.CalleId,
                        Rechazado = camion.Rechazado,
                        FechaIngreso = camion.FechaIngeso,
                        MaterialId = camion.MaterialId,
                        Calidad = camion.Calidad,
                        EsSojaEPA = camion.EsSojaEPA,
                        EsSojaIMPO = camion.EsSojaIMPO
                    };
                    callePlanta.Camiones.Add(camionPlanta);
                }

            }
            return tipoCallePlantaLista;
        }

        private bool ExisteSlotsDisponibles(CalleDto callePlayaInterna, int cantidadNuevosCamionesLlamadoEnPrebalanza)
        {
            var camionesEnPlayaInterna = servicio.ListarCallePorRecorridoPorCalleId(callePlayaInterna.Id).Count();
            var camionesLlamadosEnPreBalanza = servicio.ObtenerCantidadCamionesLlamadosEnCallePreBalanza(callePlayaInterna.Id);
            var slotsLibres = callePlayaInterna.CantidadDeCamiones - (camionesEnPlayaInterna + camionesLlamadosEnPreBalanza);
            return slotsLibres >= cantidadNuevosCamionesLlamadoEnPrebalanza;
        }

        private void EnviarMensajeLlamadoACartelPrebalanza(ResultadoMensajeCartelLed configuracionCartel)
        {
            var cartel = servicio.ObtenerConfiguracionGeneral(ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);
            servicioComandos.Ejecutar(new EnviarMensajeCartelLed
            {
                Mensaje = configuracionCartel.Mensaje,
                Codigo = cartel?.Valor,
                NumeroTrama = configuracionCartel.NumeroTrama,
                NumeroPrograma = configuracionCartel.NumeroPrograma,
                NumeroVariable = configuracionCartel.NumeroVariable,
            });
        }

        private void LimpiarMensajeLlamadoACartelPrebalanza(ResultadoMensajeCartelLedReordenado resultadoCartelLed)
        {
            var cartel = servicio.ObtenerConfiguracionGeneral(ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);
            foreach (var mensajeCartelLed in resultadoCartelLed.ListaDeMensajes)
            {
                servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                {
                    Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                    Codigo = cartel?.Valor,
                    NumeroTrama = mensajeCartelLed.Trama,
                    NumeroPrograma = mensajeCartelLed.Programa,
                    NumeroVariable = mensajeCartelLed.Variable,
                });
            }
        }

        private void AgregarErroresARespuesta(Resultado resultado, RespuestaEstandarDto response)
        {
            if (resultado.HayErrores)
            {
                foreach (var item in resultado.Errores)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }
        }
    }
}