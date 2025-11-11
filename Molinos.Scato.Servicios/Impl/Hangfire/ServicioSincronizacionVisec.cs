using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Behavior;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioSincronizacionVisec : IServicioSincronizacionVisec
    {
        private readonly IServicioRepositorio repositorio;
        private readonly ILogger log;
        private readonly IServicioComandos comandos;

        public ServicioSincronizacionVisec(IServicioRepositorio repositorio, ILogger log, IServicioComandos comandos)
        {
            this.repositorio = repositorio;
            this.log = log;
            this.comandos = comandos;
        }

        public string SincronizarEstadoTransmision()
        {
            log.Info("Iniciando sincronización de estado de transmisión VISec...");
            var transmisiones = repositorio.ListarVisecTransmisionPorEstado(EstadoTransmisionAVisec.EnviadoAVisec);
            log.Info($"Se encontraron {transmisiones.Count} transmisiones con estado 'Enviado a VISec' para sincronizar.");
            foreach (var transmision in transmisiones)
            {
                if (!string.IsNullOrEmpty(transmision.NumeroProceso))
                    ProcesarSincronizacionTransmision(transmision);
            }

            return transmisiones.Count.ToString();
        }

        private void ProcesarSincronizacionTransmision(VisecTransmisionDto transmision)
        {
            log.Info($"Procesando sincronización de transmisión ID: {transmision.Id}, Proceso: {transmision.NumeroProceso}");
            var resultadoConsulta = comandos.Ejecutar(new ConsultarEstadoTransmisionVisec
            {
                NumeroProceso = transmision.NumeroProceso
            }) as ResultadoConsultarEstadoTransmisionVisec;

            if (resultadoConsulta.HayErrores && !resultadoConsulta.Estado.HasValue)
            {
                log.Warn($"Error al consultar estado para transmisión ID: {transmision.Id} - {resultadoConsulta.Errores.FirstOrDefault().Value}");
                return;
            }

            if (resultadoConsulta.Estado.HasValue)
            {
                transmision.Estado = resultadoConsulta.Estado.Value;
                transmision.DetalleTransaccion = resultadoConsulta.HayErrores ? resultadoConsulta.Errores.FirstOrDefault().Value : string.Empty;
                log.Info($"Actualizando estado de transmisión ID: {transmision.Id} a {transmision.Estado}");
                comandos.Ejecutar(new CrearActualizarVisecTransmision
                {
                    Dto = transmision
                });
            }
        }
    }
}