using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSuscribirEventos : ProcesadorComando<SuscribirEventos>
    {
        private readonly IConfiguracionProvider config;
        private readonly IServicioOrquestador orquestador;

        public ProcesadorSuscribirEventos(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider config, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.config = config;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(SuscribirEventos comando)
        {
            Log.Info("Creando suscripciones para puestos de trabajo...");
            var resultadoComando = new Resultado();
            var urlNotificaciones = config.AppSettings["UrlNotificaciones"];
            var urlNotificacionesWeb = config.AppSettings["UrlNotificacionesWeb"];
            var puestosDeTrabajo = Repositorio.Listar<PuestoDeTrabajo>();

            foreach (var puestoDeTrabajo in puestosDeTrabajo)
            {
                Log.Debug("Suscribiendo dispositivos de puesto {0}", puestoDeTrabajo.NombrePuesto);
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.Lector, "LecturaTarjetaRecibida",
                            urlNotificacionesWeb, resultadoComando);
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.SensorQuiebre, "EntradaActivada",
                            urlNotificaciones, resultadoComando);
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.SensorQuiebre, "EntradaDesactivada",
                            urlNotificaciones, resultadoComando);
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.Lector, "ErrorConexionDispositivo",
                            urlNotificacionesWeb, resultadoComando);
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.Lector, "ConexionDispositivoCorrecta",
                            urlNotificacionesWeb, resultadoComando);
                if (!string.IsNullOrEmpty(puestoDeTrabajo.Concentrador))
                {
                    Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.Concentrador, "CambioEstadoSensor",
                                urlNotificaciones, resultadoComando);
                }
                Suscribir(puestoDeTrabajo.Id, puestoDeTrabajo.LectorQr, "LecturaQr",
                            urlNotificacionesWeb, resultadoComando);
            }

            var balanzas = Repositorio.Listar<BalanzaPuerto>();
            if(balanzas != null)
            {
                foreach (BalanzaPuerto balanza in balanzas)
                {
                    Suscribir(balanza.Id * -1, balanza.CodigoDispositivo, "BalanzadaRecibida", urlNotificaciones, resultadoComando);
                }
            }

            Log.Info("Suscripciones creadas");
            return resultadoComando;
        }

        private void Suscribir(int puestoDeTrabajoId, string codigoDispositivo, string codigoEvento, string rutaAcceso, Resultado resultadoComando)
        {
            try
            {
                Log.Debug("Creando suscripción: Dispositivo={0} Evento={1}", codigoDispositivo, codigoEvento);
                var resultado = orquestador.Suscribir(new ComandoSuscribir
                {
                    CodigoDispositivo = codigoDispositivo,
                    CodigoEvento = codigoEvento,
                    RutaAccesoSuscriptor = rutaAcceso,
                    Persistente = true
                });

                if (resultado.Mensaje.Codigo != 0)
                {
                    Log.Error("No se pudo crear la suscripción para el evento {0} del dispositivo {1}. Mensaje: {2}-{3}", codigoEvento, codigoDispositivo, resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    resultadoComando.Errores.Add(codigoEvento + puestoDeTrabajoId, resultado.Mensaje.Descripcion);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el evento {0} del dispositivo {1}.", codigoEvento, codigoDispositivo);
                resultadoComando.Errores.Add(codigoEvento + puestoDeTrabajoId, e.Message);
            }
        }
    }
}
