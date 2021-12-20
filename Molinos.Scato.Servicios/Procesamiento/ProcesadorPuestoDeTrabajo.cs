using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using Comando = Molinos.Scato.Dominio.Comandos.Comando;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorPuestoDeTrabajo<TComando> : ProcesadorComando<TComando> where TComando : Comando
    {
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IConfiguracionProvider config;

        protected ProcesadorPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador servicioOrquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log)
        {
            this.config = config;
            this.servicioOrquestador = servicioOrquestador;
        }

        protected void SuscribirDispositivos(PuestoDeTrabajoDto dto, Resultado resultado)
        {
            var urlNotificaciones = config.AppSettings["UrlNotificaciones"];
            var urlNotificacionesWeb = config.AppSettings["UrlNotificacionesWeb"];
            try
            {
                Suscribir(dto.Lector, "Lector", "LecturaTarjetaRecibida", urlNotificacionesWeb, resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el lector {0}.", dto.Lector);
                if (!resultado.Errores.Keys.Contains("Lector"))
                {
                    resultado.Error("Lector", String.Format(Textos.PuestoDeTrabajo_SuscribirLector, e.Message));
                }
            }
            try
            {
                Suscribir(dto.Lector, "Lector", "ErrorConexionDispositivo", urlNotificacionesWeb, resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el lector {0}.", dto.Lector);
                if (!resultado.Errores.Keys.Contains("Lector"))
                {
                    resultado.Error("Lector", String.Format(Textos.PuestoDeTrabajo_SuscribirLector, e.Message));
                }
            }
            try
            {
                Suscribir(dto.Lector, "Lector", "ConexionDispositivoCorrecta", urlNotificacionesWeb, resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el lector {0}.", dto.Lector);
                if (!resultado.Errores.Keys.Contains("Lector"))
                {
                    resultado.Error("Lector", String.Format(Textos.PuestoDeTrabajo_SuscribirLector, e.Message));
                }
            }
            try
            {
                Suscribir(dto.SensorQuiebre, "SensorQuiebre", "EntradaActivada", urlNotificaciones, resultado);
                Suscribir(dto.SensorQuiebre, "SensorQuiebre", "EntradaDesactivada", urlNotificaciones, resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el sensor {0}.", dto.SensorQuiebre);
                if (!resultado.Errores.Keys.Contains("SensorQuiebre"))
                {
                    resultado.Error("SensorQuiebre", String.Format(Textos.PuestoDeTrabajo_SuscribirSensor, e.Message));
                }
            }
        }

        protected void CancelarDispositivos(PuestoDeTrabajoDto dto, Resultado resultado)
        {
            var urlNotificaciones = config.AppSettings["UrlNotificaciones"];
            var urlNotificacionesWeb = config.AppSettings["UrlNotificacionesWeb"];
            try
            {
                var resultadoOrq = servicioOrquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
                    {
                        CodigoDispositivo = dto.Lector,
                        RutaAccesoSuscriptor = urlNotificacionesWeb,
                    });
                if (resultadoOrq.Mensaje.Codigo != 0)
                {
                    Log.Error("No se pudo cancelar la suscripción para el lector {0}. Mensaje: {1}-{2}", dto.Lector, resultadoOrq.Mensaje.Codigo, resultadoOrq.Mensaje.Descripcion);
                }
                // Agregar EntradaDesactivada cuando se apruebe
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo cancelar la suscripción para el lector {0}.", dto.Lector);
            }

            try
            {
                var resultadoOrq = servicioOrquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
                {
                    CodigoDispositivo = dto.SensorQuiebre,
                    RutaAccesoSuscriptor = urlNotificaciones,
                });
                if (resultadoOrq.Mensaje.Codigo != 0)
                {
                    Log.Error("No se pudo cancelar la suscripción para el sensor {0}. Mensaje: {1}-{2}", dto.SensorQuiebre, resultadoOrq.Mensaje.Codigo, resultadoOrq.Mensaje.Descripcion);
                }
                // Agregar EntradaDesactivada cuando se apruebe
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo cancelar la suscripción para el sensor {0}.", dto.SensorQuiebre);
            }

            try
            {
                var resultadoOrq = servicioOrquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
                {
                    CodigoDispositivo = dto.Concentrador,
                    RutaAccesoSuscriptor = urlNotificaciones,
                });
                if (resultadoOrq.Mensaje.Codigo != 0)
                {
                    Log.Error("No se pudo cancelar la suscripción para el sensor {0}. Mensaje: {1}-{2}", dto.Concentrador, resultadoOrq.Mensaje.Codigo, resultadoOrq.Mensaje.Descripcion);
                }
                // Agregar EntradaDesactivada cuando se apruebe
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo cancelar la suscripción para el sensor {0}.", dto.SensorQuiebre);
            }
        }


        private void Suscribir(string codigoDispositivo, string dispositivo, string codigoEvento, string rutaAcceso, Resultado resultadoComando)
        {
            var resultado = servicioOrquestador.Suscribir(new ComandoSuscribir
            {
                CodigoDispositivo = codigoDispositivo,
                CodigoEvento = codigoEvento,
                RutaAccesoSuscriptor = rutaAcceso,
                Persistente = true
            });

            if (resultado.Mensaje.Codigo != 0)
            {
                Log.Error("No se pudo crear la suscripción para el dispositivo {0}. Mensaje: {1}-{2}", codigoDispositivo, resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                if (!resultadoComando.Errores.Keys.Contains(dispositivo))
                {
                    resultadoComando.Error(dispositivo, String.Format(Textos.PuestoDeTrabajo_SuscribirLector, resultado.Mensaje.Descripcion));
                }
            }
        }
    }
}
