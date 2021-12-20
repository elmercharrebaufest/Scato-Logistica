using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSuscribirDispositivos : ProcesadorComando<SuscribirDispositivos>
    {
        private readonly IConfiguracionProvider config;
        private readonly IServicioOrquestador orquestador;

        public ProcesadorSuscribirDispositivos(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider config, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.config = config;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(SuscribirDispositivos comando)
        {
            Log.Info($"Creando suscripcion para dispositivo {comando.Codigo}");
            var resultadoComando = new Resultado();
            var urlNotificaciones = config.AppSettings["UrlNotificaciones"];
            var urlNotificacionesWeb = config.AppSettings["UrlNotificacionesWeb"];

            Suscribir(comando.Codigo, comando.Evento, comando.RutaWeb ? urlNotificacionesWeb : urlNotificaciones, resultadoComando);
            return resultadoComando;
        }

        private void Suscribir(string codigoDispositivo, string codigoEvento, string rutaAcceso, Resultado resultadoComando)
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
                    resultadoComando.Errores.Add(codigoEvento + codigoDispositivo, resultado.Mensaje.Descripcion);
                }
                Log.Debug($"Resultado de la suscripcion {resultado.Mensaje.Codigo}: {resultado.Mensaje.Descripcion}");
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo crear la suscripción para el evento {0} del dispositivo {1}.", codigoEvento, codigoDispositivo);
                resultadoComando.Errores.Add(codigoEvento + codigoDispositivo, e.Message);
            }
        }
    }
}
