using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCancelarSuscripcionDispositivos : ProcesadorComando<CancelarSuscripcionDispositivos>
    {
        private readonly IConfiguracionProvider config;
        private readonly IServicioOrquestador orquestador;

        public ProcesadorCancelarSuscripcionDispositivos(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider config, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.config = config;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(CancelarSuscripcionDispositivos comando)
        {
            Log.Info($"Cancelar suscripcion para dispositivo {comando.Codigo}");
            var resultadoComando = new Resultado();
            var urlNotificaciones = config.AppSettings["UrlNotificaciones"];
            var urlNotificacionesWeb = config.AppSettings["UrlNotificacionesWeb"];

            CancelarSuscripcion(comando.Codigo, comando.RutaWeb ? urlNotificacionesWeb : urlNotificaciones, resultadoComando);
            return resultadoComando;
        }

        private void CancelarSuscripcion(string codigoDispositivo, string rutaAcceso, Resultado resultadoComando)
        {
            try
            {
                Log.Debug("Cancelar suscripción: Dispositivo={0} Ruta={1}", codigoDispositivo, rutaAcceso);
                var resultado = orquestador.CancelarSuscripcion(new ComandoCancelarSuscripcion
                {
                    CodigoDispositivo = codigoDispositivo,
                    RutaAccesoSuscriptor = rutaAcceso
                });

                if (resultado.Mensaje.Codigo != 0)
                {
                    Log.Error("No se pudo cancelar la suscripción para el dispositivo {0}. Mensaje: {1}-{2}", codigoDispositivo, resultado.Mensaje.Codigo, resultado.Mensaje.Descripcion);
                    resultadoComando.Errores.Add(codigoDispositivo, resultado.Mensaje.Descripcion);
                }
                Log.Debug($"Resultado de cancelar suscripcion {resultado.Mensaje.Codigo}: {resultado.Mensaje.Descripcion}");
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo cancelar la suscripción para el dispositivo {0}", codigoDispositivo);
                resultadoComando.Errores.Add(codigoDispositivo, e.Message);
            }
        }
    }
}
