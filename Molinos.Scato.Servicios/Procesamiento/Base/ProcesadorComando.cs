using System;
using System.Threading;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorComando<TComando> : IProcesadorComando<TComando> where TComando : Comando
    {
        protected IRepositorio Repositorio { get; private set; }
        protected IConversor Conversor { get; private set; }
        protected ILogger Log { get; private set; }

        protected ProcesadorComando(IRepositorio repositorio, IConversor conversor, ILogger log)
        {
            Log = log;
            Repositorio = repositorio;
            Conversor = conversor;
        }

        public Resultado Ejecutar(Comando comando)
        {
            var count = 1;
            const int maxTries = 3;

            while (true)
            {
                try
                {
                    return Ejecutar((TComando)comando);
                }
                catch (Exception e)
                {
                    Thread.Sleep(count * 1500);
                    Log.Error(e, "Ocurrió un error el ejecutar el comando - Intento numero:" + count);
                    if (++count == maxTries)
                    {
                        throw;
                    }
                }
            }
        }

        public abstract Resultado Ejecutar(TComando comando);
    }
}
