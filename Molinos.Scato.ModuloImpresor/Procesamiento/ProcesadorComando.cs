using Molinos.Scato.Dominio.Comandos;
using Ninject.Extensions.Logging;
using System;
using System.Threading;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public abstract class ProcesadorComando<TComando> : IProcesadorComando<TComando> where TComando : Comando
    {
        protected ILogger Log { get; private set; }

        protected ProcesadorComando(ILogger log)
        {
            Log = log;
        }

        public Resultado Ejecutar(Comando comando)
        {
                var resultado = new Resultado();
                var count = 1;
                const int maxTries = 4;
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
                            resultado.Error("0", "Ocurrió un error el ejecutar el comando - Intento numero:" + count);
                            break;
                        }
                    }
                }
                return resultado;
            
        }

        public abstract Resultado Ejecutar(TComando comando);

    }
}
