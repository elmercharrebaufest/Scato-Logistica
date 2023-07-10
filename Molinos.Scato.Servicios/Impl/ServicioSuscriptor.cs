using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioSuscriptor : IServicioSuscriptor
    {
        private readonly IServicioComandos servicioComandos;
        private readonly ILogger log;
        private readonly IServicioRepositorio repositorio;
        private readonly IServicioEstadoPuesto estadoPuesto;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IConfiguracionProvider configuracion;

        public ServicioSuscriptor(IServicioComandos servicioComandos, ILogger log
            , IServicioRepositorio repositorio, IServicioEstadoPuesto estadoPuesto
            , IServicioOrquestador servicioOrquestador, IConfiguracionProvider configuracion)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
            this.repositorio = repositorio;
            this.estadoPuesto = estadoPuesto;
            this.servicioOrquestador = servicioOrquestador;
            this.configuracion = configuracion;
        }

        public void Recibir(NotificacionEvento notificacion)
        {
            log.Debug("Notificacion recibida: {0}", notificacion);
            try
            {
                switch (notificacion.CodigoEvento)
                {
                    case "EntradaActivada":
                        var resultadoApertura = servicioComandos.Ejecutar(new CrearMotivoQuiebreBarrera { CodigoDispositivo = notificacion.CodigoDispositivo, Apertura = true });
                        EnviarMail(Textos.MailQuiebreBarrera, resultadoApertura, notificacion);
                        break;

                    case "EntradaDesactivada":
                        var resultadoCierre = servicioComandos.Ejecutar(new CrearMotivoQuiebreBarrera { CodigoDispositivo = notificacion.CodigoDispositivo, Apertura = false });
                        EnviarMail(Textos.MailCierreBarrera, resultadoCierre, notificacion);
                        break;

                    case "BalanzadaRecibida":
                        if (notificacion.Datos["tipoBalanzada"] == "fin")
                        {
                            servicioComandos.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = notificacion.CodigoDispositivo, CodigoDispositivo = notificacion.CodigoDispositivo, Hasta = Int32.Parse(notificacion.Datos["id"]) });
                        }
                        break;

                    case "CambioEstadoSensor":
                        estadoPuesto.NotificarSensorBarrera(notificacion);
                        estadoPuesto.NotificarSensorBarreraHidraulicas(notificacion);
                        bool estado;
                        if (bool.TryParse(notificacion.Datos["Mensaje"], out estado))
                        {
                            var cierreAutomaticoBarreraActivo = configuracion.AppSettings.Get("ActivarCierreAutomaticoDeBarrera");

                            if (cierreAutomaticoBarreraActivo.ToUpper() == "TRUE")
                            {
                                EjecutarCierreDeBarreraAutomatica(notificacion, estado);
                            }

                            estadoPuesto.NotificarCambioDeEstado(notificacion.CodigoDispositivo, estado);
                        }
                        break;

                    case "LecturaCPE":
                        LecturaCartaPorteElectronica(notificacion.CodigoDispositivo, int.Parse(notificacion.Datos["QR"]));
                        break;

                    case CodigosEventos.CambioEstadoSensorCamaraALPR:
                        log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorCamaraALPR - Inicio");
                        var patente = notificacion.Datos["Patente"];
                        var hidraulicasDisponibles = repositorio.ListarHidraulicasPorEstado(EstadoHidraulica.Disponible);
                        if (!hidraulicasDisponibles.Any())
                            break;

                        var hidraulicasDiponsibleId = hidraulicasDisponibles.Select(x => x.HidraulicaId).ToList();
                        var datosDeCamion = ObtenerDatosPorPatente(patente);
                        if (datosDeCamion == null)
                            break;

                        var hidraulicaAsignadaId = datosDeCamion.HidraulicasId.FirstOrDefault(x => hidraulicasDiponsibleId.Contains(x));
                        if (hidraulicaAsignadaId == 0)
                            break;

                        var configuracionCalle = repositorio.ObtenerConfiguracionCalleHidraulicaPorSensorCamaraALPR(notificacion.CodigoDispositivo);
                        var nombreHidraulicaAsignada = hidraulicasDisponibles.Where(x => x.Id == hidraulicaAsignadaId).Select(x => x.HidraulicaNombre).FirstOrDefault();
                        var tiempoDeIntervalo = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoVolcadoras, Constantes.ConfiguracionGeneral.Volcadoras.CartelLedIntervalo);
                        log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorCamaraALPR - Cartel: {configuracionCalle.CodigoCartel} Patente: {patente} Hidraulica: {nombreHidraulicaAsignada}");
                        EnviarMensajeACartelConIntervalo(configuracionCalle.CodigoCartel, patente, nombreHidraulicaAsignada, (tiempoDeIntervalo != null) ? int.Parse(tiempoDeIntervalo.Valor) : 3000);
                        ActualizarEstadoHidraulica(hidraulicaAsignadaId, EstadoHidraulica.Llamando, patente, configuracionCalle.CodigoCartel);
                        log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorCamaraALPR - Fin");
                        break;

                    case CodigosEventos.CambioEstadoSensorGeneral:
                        log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorGeneral - Inicio");
                        if (Enum.TryParse(notificacion.Datos["Accion"], out TipoAccionSensor tipoAccion))
                        {
                            switch (tipoAccion)
                            {
                                case TipoAccionSensor.CamionCruzo:
                                    var configuracionCalleHidraulica = repositorio.ObtenerConfiguracionCalleHidraulicaPorSensorCirculacion(notificacion.CodigoDispositivo);
                                    log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorGeneral - CamionCruzo - Cartel: {configuracionCalleHidraulica.CodigoCartel}");
                                    LimpiarMensajeCartel(configuracionCalleHidraulica.CodigoCartel);
                                    log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorGeneral - Fin");
                                    break;

                                case TipoAccionSensor.HidraulicaBajo:
                                    var hidraulica = repositorio.ObtenerHidraulicaPorSensorBajada(notificacion.CodigoDispositivo);
                                    log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorGeneral - HidraulicaBajo - Hidraulica: {hidraulica.Nombre}");
                                    ActualizarEstadoHidraulica(hidraulica.Id, EstadoHidraulica.Disponible, string.Empty, string.Empty);
                                    log.Debug($"LlamadoAutomaticoVolcables - Evento CambioEstadoSensorGeneral - Fin");
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo procesar la notificacion");
                throw;
            }
            log.Debug($"Notificacion {notificacion.CodigoEvento} procesada con exito");
        }

        private void EnviarMail(string cuerpo, Resultado resultadoApertura, NotificacionEvento notificacion)
        {
            if (!resultadoApertura.HayErrores)
            {
                var ultimosMovimientos = repositorio.ObtenerUltimosMovimientosDispositivo(notificacion.CodigoDispositivo);
                var ultMovString = "";

                foreach (MotivoQuiebreBarreraDto motivo in ultimosMovimientos)
                {
                    ultMovString += (motivo.Apertura == true ? "Quiebre " : "Cierre ") + motivo.Fecha + motivo.Hora + "<br/>";
                }

                var usuariosApertura = repositorio.ObtenerUsuariosQuiebreApertura();
                var resultado = resultadoApertura as ResultadoMotivoQuiebre;
                byte[] foto = null;
                if (resultado.Fotos.Any())
                {
                    foto = repositorio.ObtenerFotoPorQuiebreDeBarrera(resultado.Fotos.First(), DateTime.Now);
                }
                servicioComandos.Ejecutar(new EnvioMail { Destinatarios = usuariosApertura, Titulo = "Quiebre de Barrera", Cuerpo = string.Format(cuerpo, notificacion.CodigoDispositivo, DateTime.Now, notificacion.CodigoDispositivo, ultMovString, Convert.ToBase64String(foto)) });
            }
        }

        private void LecturaCartaPorteElectronica(string dispositivo, int ctg)
        {
            var puesto = repositorio.ObtenerPuestoDeTrabajoPorDispositivo(dispositivo);
            servicioComandos.Ejecutar(new NotificarLecturaCPE { PuestoId = puesto.Id, CentroId = puesto.CentroId, NroCtg = ctg });
        }

        private void EjecutarCierreDeBarreraAutomatica(NotificacionEvento notificacion, bool estado)
        {
            var configuraciones = servicioOrquestador.ObtenerConfiguracionGrupoBarreraPorSegundoCruce(notificacion.CodigoDispositivo);
            if (configuraciones.Length == 0)
                return;

            repositorio.ActualizarDispositivoLog(notificacion.CodigoDispositivo, "EstadoSensor", estado.ToString(), false);

            var gruposBarrera = configuraciones.Select(q => q.AgrupadorCodigo).ToList();
            var gruposEnUso = repositorio.ObtenerGruposBarreraEnUso(gruposBarrera).Distinct();
            foreach (var grupo in gruposEnUso)
            {
                var grupoBarrera = configuraciones.FirstOrDefault(q => q.AgrupadorCodigo == grupo);

                var dispositivoCodigoLista = new List<string>
                {
                    grupoBarrera.SensorArribaCodigo,
                    grupoBarrera.SensorAbajoCodigo,
                    grupoBarrera.SensorSegundoCruceCodigo
                };

                var logEstadoSensor = repositorio.ObtenerLogDispositivos(dispositivoCodigoLista);

                if (logEstadoSensor.Count() < dispositivoCodigoLista.Count)
                    return;

                var sensorArriba = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == grupoBarrera.SensorArribaCodigo);
                var sensorAbajo = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == grupoBarrera.SensorAbajoCodigo);
                var sensorSegundoCruce = logEstadoSensor.FirstOrDefault(q => q.CodigoDispositivo == grupoBarrera.SensorSegundoCruceCodigo);

                var estadoSensorArriba = false;
                var estadoSensorAbajo = false;
                var estadoSensorCruce = false;
                var estadoSensorCruceAnterior = false;

                if (bool.TryParse(sensorArriba.ValorActual, out estadoSensorArriba)
                    && bool.TryParse(sensorAbajo.ValorActual, out estadoSensorAbajo)
                    && bool.TryParse(sensorSegundoCruce.ValorActual, out estadoSensorCruce))
                {
                    if (bool.TryParse(sensorSegundoCruce.ValorAnterior, out estadoSensorCruceAnterior))
                    {
                        if (estadoSensorArriba == true && estadoSensorAbajo == false)
                        {
                            if (estadoSensorCruceAnterior == true && estadoSensorCruce == false)
                            {
                                //log.Debug("Se ejecutara cierre de barrera automatico con el codigo: " + grupoBarrera.BarreraAbajoCodigo);
                                var resultadoEjecutarCierreBarrera = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                                {
                                    CodigoDispositivo = grupoBarrera.BarreraAbajoCodigo
                                });
                                //log.Debug("Se ejecuto cierre de barrera automatico con el codigo: " + grupoBarrera.BarreraAbajoCodigo);
                                repositorio.ActualizarDispositivoLog(grupoBarrera.SensorSegundoCruceCodigo, "EstadoSensor", "", true);
                            }
                        }
                    }
                }
            }
        }

        private CamionHidraulicaDto ObtenerDatosPorPatente(string patente)
        {
            CamionHidraulicaDto datosCamion = null;
            try
            {
                var recorrido = repositorio.ObtenerRecorridoActivoPorPatente(patente);
                if (recorrido != null)
                {
                    datosCamion = new CamionHidraulicaDto()
                    {
                        Patente = patente,
                        HidraulicasId = recorrido.HidraulicasId,
                        RecorridoId = recorrido.Id
                    };
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se obtener datos por patente {0}", patente);
            }
            return datosCamion;
        }

        private void EnviarMensajeACartelConIntervalo(string codigoCartel, string mensaje, string mensajeSecundario, int intervaloMilliseconds)
        {
            try
            {
                var mensajeCartel = repositorio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoAutomaticoVolcadoras);
                if (!string.IsNullOrEmpty(codigoCartel) && mensaje != null)
                {
                    servicioOrquestador.Ejecutar(new DetenerMensajeIntervalo
                    {
                        CodigoDispositivo = codigoCartel
                    });

                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = mensaje,
                        Codigo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = mensajeCartel.Trama,
                        NumeroVariable = mensajeCartel.Variable,
                        SegundosDeEspera = mensajeCartel.SegundosDeEspera,
                        EsMensajeConIntervalo = true,
                        MensajeSecundario = mensajeSecundario,
                        IntervaloMilliseconds = intervaloMilliseconds
                    });
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private void LimpiarMensajeCartel(string codigoCartel)
        {
            try
            {
                var mensajeCartel = repositorio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoAutomaticoVolcadoras);
                if (!string.IsNullOrEmpty(codigoCartel))
                {
                    servicioOrquestador.Ejecutar(new DetenerMensajeIntervalo
                    {
                        CodigoDispositivo = codigoCartel
                    });

                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = "PARE AQUI",
                        Codigo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = CartelTramaPare.LlamadoAutomaticoVolcadoras,
                        NumeroVariable = mensajeCartel.Variable
                    });
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private void ActualizarEstadoHidraulica(int hidraulicaId, EstadoHidraulica nuevoEstado, string patenteLlamada,string codigoCartel)
        {
            try
            {
                servicioComandos.Ejecutar(new ActualizarLlamadoAutomaticoHidraulica
                {
                    Id = hidraulicaId,
                    Estado = nuevoEstado,
                    Patente = patenteLlamada,
                    Cartel = codigoCartel
                });
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo actualizar el estado de hidraulica");
            }
        }
    }
}