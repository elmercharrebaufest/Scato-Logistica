using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.ServicioHub.Client;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.Scato.Web.Firmware
{
    public abstract class FirmwareBase : IFirmware
    {
        protected readonly ILogger log;
        protected readonly IServicioRepositorio servicio;
        protected readonly IListaDeWorkflows workflows;
        protected readonly IServicioComandos comandos;
        protected readonly IServicioOrquestador servicioOrquestador;
        protected readonly IServicioActividadFactory<IEjecutarService> factory;
        protected readonly IRecorridoWorkflow recorridoWorkflow;
        protected readonly IHubClient hubClientNotificar;
        protected readonly IHubClient hubClientLectura;
       
        public FirmwareBase(
            ILogger log, 
            IServicioRepositorio servicioRepositorio, 
            IListaDeWorkflows workflows,
            IServicioComandos comandos,
            IServicioOrquestador servicioOrquestador,
            IServicioActividadFactory<IEjecutarService> factory, 
            HubClients hubClients,
            IRecorridoWorkflow recorridoWorkflow)
        {
            this.log = log;
            this.servicio = servicioRepositorio;
            this.workflows = workflows;
            this.comandos = comandos;
            this.servicioOrquestador = servicioOrquestador;
            this.factory = factory;
            this.recorridoWorkflow = recorridoWorkflow;
            this.hubClientLectura = hubClients.Lectura;
            this.hubClientNotificar = hubClients.Notificar;
        }

        public string Ejecutar(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            return ProcesarEvento(lecturaPuestoDeTrabajo);
        }

        public abstract string ProcesarEvento(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo);

        protected virtual DatosRecorridoDto ObtenerRecorrido(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            var recorrido = servicio.ObtenerDatosRecorridoActivoPorTarjetaOPatente(lecturaPuestoDeTrabajo.TipoIdentificacion, lecturaPuestoDeTrabajo.Patente, lecturaPuestoDeTrabajo.NumeroDeTarjeta);
            if (recorrido == null)
            {
                var datosDeRecorrido = new DatosRecorridoDto
                {
                    CentroCodigoSap = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                    SinRecorrido = true
                };
                return datosDeRecorrido;
            }

            log.Info("Recorrido activo encontrado. InstanciaWorkflow: {0}, Patente: {1}, ProximaAccion: {2}", recorrido.InstanciaWorkflow, recorrido.Patente, recorrido.ProximaAccion);
            var proximaActividad = workflows.ObtenerWorkflowProximaAccion(recorrido.InstanciaWorkflow);
            if (proximaActividad == null || string.IsNullOrEmpty(proximaActividad.ProximaAccion))
                throw new InvalidOperationException($"No se ha encontrado una proxima accion para el recorrido: {recorrido.InstanciaWorkflow}");

            log.Info(JsonConvert.SerializeObject(proximaActividad));
            recorrido.ProximaAccion = proximaActividad.ProximaAccion;
            recorrido.ProximaAccionMensaje = proximaActividad.Mensaje;

            return recorrido;
        }

        protected ValidarProximaAccionPorPuestoDto ValidarProximaActividad(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            var puestos = new List<PuestoDeTrabajoDto>
            {
                new PuestoDeTrabajoDto 
                {
                    Lectura = recorrido.TarjetaDeAcceso,
                    Id = lecturaPuestoDeTrabajo.PuestoDeTrabajoId 
                }
            };
            return servicio.ValidarProximaActividadPorPuesto(recorrido, recorrido.ProximaAccion, puestos, ConfigurationManager.AppSettings["Reportes.Username"]);
        }

        protected string EjecutarPuestoDesatendido(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            try
            {
                log.Debug("Validando puesto sin patente. Tarjeta: {0} Puesto: {1}", lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.PuestoDeTrabajoId);
                if (!recorrido.SinRecorrido && recorrido.AdvertirCaladoEnPlanta)
                {
                    lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.CaladoEnPlanta_Advertencia, recorrido.Patente);
                    NotificarMensajeErrorPorSignalR(lecturaPuestoDeTrabajo, TipoAlerta.Advertencia);
                }

                var resultado = ValidarProximaActividad(lecturaPuestoDeTrabajo, recorrido);
                if ((!recorrido.SinRecorrido && !string.IsNullOrEmpty(resultado.NumeroDocumentoIngreso) && !string.IsNullOrEmpty(resultado.Patente)) || recorrido.SinRecorrido)
                    EjecutarDispositivos(lecturaPuestoDeTrabajo, recorrido);

                if (!resultado.Valida)
                {
                    lecturaPuestoDeTrabajo.TarjetaValida = false;
                    lecturaPuestoDeTrabajo.MensajeError = resultado.MensajeError;
                    return resultado.MensajeError;
                }
                
                log.Debug("Ejecutando puesto sin patente. Tarjeta: {0} Puesto: {1} Puesto: {2} Actividad: {3}", lecturaPuestoDeTrabajo.NumeroDeTarjeta, resultado.PuestoDeTrabajoId, resultado.ProximaActividad);
                var serviciowf = factory.CrearServicio(resultado.WorkflowDefinicionId);
                var resultadoActividad = serviciowf.Ejecutar(resultado.InstanceId, new ControlRecorridoDto
                {
                    WorkflowInstanceId = resultado.InstanceId,
                    NombreUsuario = String.Empty,
                    Actividad = Textos.ResourceManager.GetString("Act" + resultado.ProximaActividad) ?? resultado.ProximaActividad,
                    ActividadXaml = resultado.ProximaActividad,
                    Decision = true,
                    PuestoDeTrabajoId = resultado.PuestoDeTrabajoId
                });

                var huboErrorEnWorkflow = resultadoActividad != null && resultadoActividad.HayErrores;
                if (huboErrorEnWorkflow) log.Error("La ejecución de la actividad {0} terminó con errores: {1}", resultado.ProximaActividad, resultadoActividad.Errores.First().Value);
                return huboErrorEnWorkflow ? resultadoActividad.Errores.First().Value : Constantes.ResultadoProcesoIdentificacionVehicular.EjecucionExitosa;
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
                return e.Message;
            }
        }

        protected void NotificarMensajeErrorPorSignalR(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, TipoAlerta tipo = TipoAlerta.Error)
        {
            var notificacionDto = new NotificacionDto
            {
                Hora = DateTime.Now,
                Grupo = lecturaPuestoDeTrabajo.CentroId + "|" + lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
                Mensaje = lecturaPuestoDeTrabajo.MensajeError,
                TipoAlerta = tipo
            };
            NotificarPorSignalR(notificacionDto);
        }

        protected void NotificarPorSignalR(NotificacionDto notificacionDto)
        {
            try
            {
                log.Debug("Iniciando conexion signalR");

                comandos.Ejecutar(new CrearNotificacion { Dto = notificacionDto });

                hubClientNotificar.Invoke("Notificar", notificacionDto);

                log.Debug("Fin- Mensaje enviado a usuario: {0} exitosamente", notificacionDto.Grupo);

            }
            catch (Exception e)
            {
                log.Error(e, "Error al enviar notificación: {0}", notificacionDto.Mensaje);
            }
        }

        protected  void NotificarLecturaPorSignalR(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            try
            {
                log.Debug($"Lectura de Puesto de Trabajo : {JsonConvert.SerializeObject(lecturaPuestoDeTrabajo)}");
                InvokeNotificarLectura(lecturaPuestoDeTrabajo);
                log.Debug("Fin - Iniciando conexion signalR");
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la Notificacion via SignalR para: {0}", lecturaPuestoDeTrabajo.CodigoDispositivo);
            }
        }

        protected void EjecutarDispositivos(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido)
        {
            try
            {
                log.Debug($"Validando puesto con patente. Tarjeta: {lecturaPuestoDeTrabajo.NumeroDeTarjeta} Puesto: {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}");
                if (lecturaPuestoDeTrabajo.VideoCamaras != null && lecturaPuestoDeTrabajo.VideoCamaras.Any())
                {
                    var date = DateTime.Now;
                    foreach (var videoCamara in lecturaPuestoDeTrabajo.VideoCamaras)
                    {
                        date = TomarFoto(lecturaPuestoDeTrabajo, recorrido, date, videoCamara);
                    }
                }

                foreach (var dispositivo in lecturaPuestoDeTrabajo.Entrada)
                {
                    log.Debug("Ejecutando Barrera de entrada {1} para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId, dispositivo);
                    var resultado = servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera
                    {
                        CodigoDispositivo = dispositivo
                    });

                    if (resultado.Mensaje.Codigo != 0)
                    {
                        log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                    }
                }

                foreach (var dispositivo in lecturaPuestoDeTrabajo.Salida)
                {
                    log.Debug("Ejecutando cierre de Barrera {1} para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId, dispositivo);
                    var resultado = servicioOrquestador.Ejecutar(new EjecutarCierreBarrera
                    {
                        CodigoDispositivo = dispositivo
                    });

                    if (resultado.Mensaje.Codigo != 0)
                    {
                        log.Error("Fallo el Cierre del dispositivo: {0}", resultado.Mensaje.Descripcion);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la ejecucion del workflow relacionado con la tarjeta: {0}", lecturaPuestoDeTrabajo.NumeroDeTarjeta);
            }
        }

        private DateTime TomarFoto(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido, DateTime date, VideoCamaraDto videoCamara)
        {
            try
            {
                log.Debug("Ejecutando Camara para el puesto: {0}", lecturaPuestoDeTrabajo.PuestoDeTrabajoId);
                var fotoTemporal = recorrido.SinRecorrido;
                var fileName = fotoTemporal ?
                        FotoCamionHelper.GenerarNombreTemporal(lecturaPuestoDeTrabajo.NumeroDeTarjeta, date)
                        : FotoCamionHelper.GenerarNombre(recorrido.CentroCodigoSap, recorrido.NumeroDocumentoIngreso, recorrido.Patente, recorrido.ProximaAccion, date, TipoVehiculo.Camión);
                date = date.AddSeconds(1);
                var resultado = servicioOrquestador.Ejecutar(
                    new EjecutarTomarFoto
                    {
                        CodigoDispositivo = videoCamara.Codigo,
                        FilePath = videoCamara.Directorio,
                        SubPath = fotoTemporal ? "temp" : DateTime.Today.ToString("yyyyMMdd"),
                        FileName = fileName
                    });
                var resultadoConPatente = resultado as ResultadoObtenerPatente;
                if (resultadoConPatente != null)
                {
                    CargarPatenteEnResultado(lecturaPuestoDeTrabajo, recorrido, fileName, resultadoConPatente);
                }

                if (resultado.Mensaje.Codigo != 0)
                {
                    log.Error("Fallo la Apertura del dispositivo: {0}", resultado.Mensaje.Descripcion);
                }

            }
            catch (Exception e)
            {
                log.Error(e, "Fallo la Apertura del dispositivo: {0}", lecturaPuestoDeTrabajo.Entrada);
            }

            return date;
        }

        private void CargarPatenteEnResultado(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, DatosRecorridoDto recorrido, string fileName, ResultadoObtenerPatente resultadoConPatente)
        {
            lecturaPuestoDeTrabajo.PatenteLeida = resultadoConPatente.Patente;
            lecturaPuestoDeTrabajo.OcrActivo = true;
            var resultadoPatente = comandos.Ejecutar(
                new ActualizarLecturaDeTarjetaPatente
                {
                    PuestoDeTrabajoId = lecturaPuestoDeTrabajo.PuestoDeTrabajoId,
                    Lectura = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                    PatenteLeida = resultadoConPatente.Patente,
                    Patente = recorrido.Patente
                });
            if (!resultadoPatente.HayErrores)
            {
                lecturaPuestoDeTrabajo.ReconocimientoExitoso = ((ResultadoValidarPatente)resultadoPatente).ReconocimientoExitoso;
            }
            log.Debug($"Patente leída en el puesto {lecturaPuestoDeTrabajo.PuestoDeTrabajoId}({fileName}): {resultadoConPatente.Patente}, ");
        }

        protected virtual void InvokeNotificarLectura(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
        {
            hubClientLectura.Invoke("NotificarLectura", lecturaPuestoDeTrabajo);
        }

    }
}


