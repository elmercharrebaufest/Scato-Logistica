using System.Threading;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorImpresionAsync<TComando> : IProcesadorComando<TComando> where TComando : Comando
    {
        protected IRepositorio Repositorio { get; set; }
        protected IConversor Conversor { get; set; }
        protected ILogger Log { get; private set; }
        protected IServicioImpresorFactory ServicioImpresionFactory { get; }

        protected ProcesadorImpresionAsync(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
        {
            Repositorio = repositorio;
            Conversor = conversor;
            Log = log;
            ServicioImpresionFactory = servicioImpresion;
        }

        public Resultado Ejecutar(Comando comando)
        {
            return Ejecutar((TComando)comando);
        }

        public Resultado Ejecutar(TComando comando)
        {
            Task.Run(() => EjecutarConLoop(comando));
            
            var id = EjecutarSync(comando);
            return new ResultadoCrear {Id = id};
        }

        private void EjecutarConLoop(TComando comando)
        {
            var count = 1;
            const int maxTries = 4;
            var servicioImpresor = ServicioImpresionFactory.CrearServicio();
            while (true)
            {
                try
                {
                    EjecutarAsync(comando, servicioImpresor);
                    break;
                }
                catch
                {
                    Thread.Sleep(count * 1000);
                    try
                    {
                        Log.Error("Error al imprimir, reintento numero:" + count);
                    }
                    catch { }
                    if (++count == maxTries)
                    {
                        break;
                    }
                }
            }    
        }
        protected abstract void EjecutarAsync(TComando comando, IServicioImpresion servicioImpresor);
        protected abstract int EjecutarSync(TComando comando);

    }
}
