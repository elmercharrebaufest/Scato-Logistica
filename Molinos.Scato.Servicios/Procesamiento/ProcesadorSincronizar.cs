using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorSincronizar<TComando> : ProcesadorComando<TComando> where TComando : ComandoSincronizar
    {
        protected readonly ZSDWS_SCATO ServicioSap;
        protected readonly DateTime MinimaFechaEjecucion = new DateTime(1900, 01, 01);

        protected ProcesadorSincronizar(IRepositorio repositorio, IConversor conversor, ZSDWS_SCATO servicioSap, ILogger log)
            : base(repositorio, conversor, log)
        {
            ServicioSap = servicioSap;
        }

        public override Resultado Ejecutar(TComando comando)
        {
            Resultado resultado = null;
            var nombreInterface = NombreInterface();
            var ultimaEjecución =
                Repositorio.ObtenerMasReciente<LogSincronizacion>(x => x.NombreInterface == nombreInterface && x.Completa && x.Correcta, x => x.FechaEjecucion) ??
                new LogSincronizacion
                    {
                        NombreInterface = nombreInterface,
                        FechaEjecucion = MinimaFechaEjecucion //Fecha de ejecución para que sincrnice el histórico
                    };

            Log.Info("Iniciando sincronización de la interface {0}. La última ejecución exitosa fue en {1}",
                     ultimaEjecución.NombreInterface, ultimaEjecución.FechaEjecucion);

            var logSincronizacion = new LogSincronizacion
                {
                    FechaEjecucion = DateTime.Now,
                    NombreInterface = nombreInterface
                };
            try
            {
                var cantidad = ProcesarSincronizacion(comando, ultimaEjecución.FechaEjecucion, out resultado);
                Log.Info("ProcesarSincronizacion Finalizada sincronización de la interface {0}.", nombreInterface);
                logSincronizacion.Correcta = true;
                logSincronizacion.Mensaje = string.Format("Procesados {0} elementos", cantidad);
                if (comando.Cuit == null)
                {
                    logSincronizacion.Completa = true;
                }
                if(cantidad > 0)
                {
                    Repositorio.GuardarCambios();
                }
                Log.Info("Finalizada sincronización de la interface {0}. Procesados {1} registros.",
                         ultimaEjecución.NombreInterface, cantidad);
            }
            catch (Exception e)
            {
                Log.Error(e,
                          "Error al sincronizar los datos de SAP de la interface {0}. Fecha ultima sincronización: {1}",
                          ultimaEjecución.NombreInterface, ultimaEjecución.FechaEjecucion);
                resultado.Error("", "Error al sincronizar: " + e.Message);
                logSincronizacion.Mensaje = e.Message;
            }
            finally
            {
                Repositorio.Agregar(logSincronizacion);
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        protected abstract int ProcesarSincronizacion(TComando comando, DateTime fechaUltimaEjecucion, out Resultado resultado);

        protected abstract string NombreInterface();

        protected static string MascaraCuit(string entrada)
        {
            if (entrada.Length != 11)
            {
                return entrada;
            }
            return entrada.Substring(0, 2) + "-" + entrada.Substring(2, 8) + "-" + entrada.Substring(10);
        }
    }
}