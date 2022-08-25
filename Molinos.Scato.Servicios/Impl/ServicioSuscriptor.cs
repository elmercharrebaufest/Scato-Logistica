using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        bool estado;
                        log.Info("CambioEstadoSensor " + notificacion.Datos["Mensaje"]);
                        if (bool.TryParse(notificacion.Datos["Mensaje"], out estado))
                        {
                            var simularTurnoActivoCircular = configuracion.AppSettings.Get("ActivarCierreAutomaticoDeBarrera");

                            if (simularTurnoActivoCircular.ToUpper() == "TRUE")
                            {
                                log.Info("Entro a cambio de estado de sensor");
                                repositorio.ActualizarDispositivoLog(notificacion.CodigoDispositivo, "EstadoSensor", estado.ToString(), false);
                                EjecutarCierreDeBarreraAutomatica(notificacion.CodigoDispositivo);
                            }

                            estadoPuesto.NotificarCambioDeEstado(notificacion.CodigoDispositivo, estado);
                        }
                        //else
                        //{
                        //    estadoPuesto.NotificarCambioDeEstado(notificacion.CodigoDispositivo, notificacion.Datos["Mensaje"]);
                        //}
                        break;

                    case "LecturaCPE":
                        LecturaCartaPorteElectronica(notificacion.CodigoDispositivo, int.Parse(notificacion.Datos["QR"]));
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

        private void EjecutarCierreDeBarreraAutomatica(string codigoSensor)
        {
            var configuraciones = servicioOrquestador.ObtenerConfiguracionGrupoBarrera(codigoSensor);

            if (configuraciones.Length == 0)
                return;

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
                                log.Info("Se ejecuto cierrere de barrera automatico");
                                var resultadoEjecutarCierreBarrera = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                                {
                                    CodigoDispositivo = grupoBarrera.BarreraAbajoCodigo
                                });
                                repositorio.ActualizarDispositivoLog(grupoBarrera.SensorSegundoCruceCodigo, "EstadoSensor", "", true);
                            }
                        }
                    }
                }
            }
        }
    }
}