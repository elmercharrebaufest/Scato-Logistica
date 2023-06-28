using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Molinos.Scato.WebMobile.Helpers;
using Molinos.Scato.WebMobile.Seguridad;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        }

        public ActionResult Index()
        {
            var tipoCallePlantaLista = ObtenerTiposDeCallesPlanta();
            ViewBag.LlamadoAutomaticoPrebalanza = ObtenerConfiguracionLlamadoAutomaticoPrebalanza();
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

                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores)
                    {
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
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

                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores)
                    {
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
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
                if (primerCamionEnPrebalanza.CalleRecorridoId == null)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"El camion {primerCamionEnPrebalanza.Patente} no tiene una calle de Playa Interna asignada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var callePlayaInterna = servicio.ObtenerCalle(primerCamionEnPrebalanza.CalleRecorridoId.Value);
                if (!ExisteSlotsDisponibles(callePlayaInterna))
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
                    Codigo = CodigoMensajeCartelLed.CartelPreBalanza,
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
                if (!callePreBalanza.FechaLLamada.HasValue)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"La {callePreBalanza.Nombre} no está siendo llamada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var camionesEnPrebalanza = servicio.ListarCallePorRecorridoPorCalleId(callePrebalanzaId);
                if (!camionesEnPrebalanza.Any())
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"No hay camiones en la {callePreBalanza.Nombre}.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var primerCamionEnPrebalanza = camionesEnPrebalanza.OrderBy(x => x.FechaIngeso).FirstOrDefault();
                if (primerCamionEnPrebalanza.CalleRecorridoId == null)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = $"El camion {primerCamionEnPrebalanza.Patente} no tiene una calle de Playa Interna asignada.", TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                servicioComandos.Ejecutar(new EliminarCallePreBalanzaPlayaInterna()
                {
                    CallePlayaInternaId = primerCamionEnPrebalanza.CalleRecorridoId.Value,
                    CallePreBalanzaId = callePrebalanzaId
                });

                var resultadoLimpiarCartelLed = servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
                {
                    Codigo = CodigoMensajeCartelLed.CartelPreBalanza,
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

        [HttpPost]
        public JsonResult GuardarPasoDirecto(int materialIdPaseDirecto, int calleIdPaseDirecto = 0)
        {
            var response = new RespuestaEstandarDto();

            var filaPrioritaria = servicio.ObtenerCallePrioritaria();
            if (PermisosHelper.Is(PermisosScato.AbmCalle))
            {
                if(filaPrioritaria != null && calleIdPaseDirecto == 0)
                {
                    ActualizarFilaPrebalanzaPrioritaria(filaPrioritaria, false);
                    return Json(response);
                } else if(filaPrioritaria != null && calleIdPaseDirecto > 0)
                {
                    if(filaPrioritaria.Id != calleIdPaseDirecto)
                    {
                        ActualizarFilaPrebalanzaPrioritaria(filaPrioritaria, false);
                        var filaSeleccionada = servicio.ObtenerCalle(calleIdPaseDirecto);
                        ActualizarFilaPrebalanzaPrioritaria(filaSeleccionada, true, materialIdPaseDirecto);
                    } else
                        ActualizarFilaPrebalanzaPrioritaria(filaPrioritaria, filaPrioritaria.EsPasoDirecto, materialIdPaseDirecto);
                    return Json(response);
                }
                else if(filaPrioritaria == null && calleIdPaseDirecto > 0)
                {
                    var filaSeleccionada = servicio.ObtenerCalle(calleIdPaseDirecto);
                    ActualizarFilaPrebalanzaPrioritaria(filaSeleccionada, true, materialIdPaseDirecto);
                }
                else
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = Textos.Calle_NoExisteCallePrioritaria, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            else if (PermisosHelper.Is(PermisosScato.EdicionConfiguracionPrebalanza))
            {
                if (filaPrioritaria == null)
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = Textos.Calle_NoExisteCallePrioritaria, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                else
                    ActualizarFilaPrebalanzaPrioritaria(filaPrioritaria, filaPrioritaria.EsPasoDirecto, materialIdPaseDirecto);
            }
            else
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = Textos.Permiso_NoTienePermiso, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });

            return Json(response);
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
                    EsPasoDirecto = calle.EsPasoDirecto,
                    MaterialId = calle.MaterialId,
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
                if (calle.TipoCalle == TipoCalle.PreBalanzaGranos && calle.Bloqueada && callePlanta.Camiones.Count == 0)
                {
                    DesbloquearCalle(calle.Id);
                }
            }
            return tipoCallePlantaLista;
        }

        private CalleDto ObtenerTiposDeCallesPlantaPorId(int idCalle)
        {
            var centro = ClaimsPrincipal.Current.GetUserClaim("CentroId");
            var centroId = int.Parse(centro.Value);
            var calles = servicio.ObtenerCallesPorCentro(centroId).Where(x => listaCalles.Contains(x.TipoCalle) && !x.Deshabilitada).OrderBy(x => x.Posicion).ToList();

            CalleDto callePorId = calles.Where(c => c.Id == idCalle).First();

            return callePorId;
        }

        private void DesbloquearCalle(int calleId)
        {
            try
            {
                var calle = servicio.ObtenerCalle(calleId);
                calle.Bloqueada = false;
                calle.FechaLLamada = null;
                servicioComandos.Ejecutar(new ModificarCalle { Dto = calle });
            }
            catch (Exception e)
            {
                log.Error(e, "Error al desbloquear calle PreBalanzaGranos");
            }
        }

        private bool ObtenerConfiguracionLlamadoAutomaticoPrebalanza()
        {
            var configuracionLlamadoAutomaticoPrebalanza = servicio.ObtenerConfiguracionGeneral(ConfiguracionGeneral.Pantalla.EstadoPlayaInterna, ConfiguracionGeneral.PreBalanza.LlamadoAutomatico);
            if (configuracionLlamadoAutomaticoPrebalanza == null || string.IsNullOrEmpty(configuracionLlamadoAutomaticoPrebalanza.Valor))
                return false;

            bool.TryParse(configuracionLlamadoAutomaticoPrebalanza.Valor, out bool llamadoAutomaticoPrebalanza);
            return llamadoAutomaticoPrebalanza;
        }

        private bool ExisteSlotsDisponibles(CalleDto callePlayaInterna)
        {
            var camionesEnPlayaInterna = servicio.ListarCallePorRecorridoPorCalleId(callePlayaInterna.Id).Count();
            var camionesLlamadosEnPreBalanza = servicio.ObtenerCantidadCamionesEnCallePreBalanza(callePlayaInterna.Id);
            var slotsLibres = callePlayaInterna.CantidadDeCamiones - (camionesEnPlayaInterna + camionesLlamadosEnPreBalanza);
            var slotNecesario = int.Parse(ConfigurationManager.AppSettings["SlotNecesariosLlamadaPreBalanza"]);
            return slotsLibres >= slotNecesario;
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

        private void ActualizarFilaPrebalanzaPrioritaria(CalleDto calle, bool esPasoDirecto, int? materialId = null)
        {
            calle.EsPasoDirecto = esPasoDirecto;
            if(materialId != null)
                calle.MaterialId = materialId.Value;
          
            servicioComandos.Ejecutar(new ModificarCalle
            {
                Dto = calle,
            });
        }
    }
}