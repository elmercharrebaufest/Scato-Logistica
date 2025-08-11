using System;
using System.Threading;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorImpresionAsync<TComando> : IProcesadorComando<TComando> where TComando : Comando
    {
        protected const bool DebeImprimirPorDefecto = true;
        protected const bool EsFlujoAlternoPorDefecto = false;
        protected IRepositorio Repositorio { get; set; }
        protected IConversor Conversor { get; set; }
        protected ILogger Log { get; private set; }
        protected IServicioImpresorFactory ServicioImpresorFactory { get; }

        protected ProcesadorImpresionAsync(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresorFactory)
        {
            this.Repositorio = repositorio;
            this.Conversor = conversor;
            this.Log = log;
            this.ServicioImpresorFactory = servicioImpresorFactory;
        }

        public Resultado Ejecutar(Comando comando)
        {
            return Ejecutar((TComando)comando);
        }

        public Resultado Ejecutar(TComando comando)
        {
            if (this.DebeImprimir(comando))
                Task.Run(() => this.EjecutarConLoop(comando));

            int id = 0;
            if (this.EsFlujoAlterno(comando))
                id = this.EjecutarFlujoAlternoSync(comando);
            else
                id = this.EjecutarSync(comando);

            return new ResultadoCrear { Id = id };
        }

        private void EjecutarConLoop(TComando comando)
        {
            var count = 1;
            const int maxTries = 4;
            var servicioImpresor = this.ServicioImpresorFactory.CrearServicio();

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
                    catch
                    { }

                    if (++count == maxTries)
                        break;
                }
            }
        }

        protected abstract void EjecutarAsync(TComando comando, IServicioImpresion servicioImpresor);

        protected abstract int EjecutarSync(TComando comando);

        protected virtual int EjecutarFlujoAlternoSync(TComando comando) => 0;

        protected bool DebeImprimir(TComando comando)
        {
            bool debeImprimir = DebeImprimirPorDefecto;

            try
            {
                debeImprimir = this.DoDebeImprimir(comando);
            }
            catch (Exception)
            {
                Log.Error("Error al determinar si se debe imprimir el comando" + typeof(TComando));
            }

            return debeImprimir;
        }

        protected virtual bool DoDebeImprimir(TComando comando) => DebeImprimirPorDefecto;
        protected virtual bool EsFlujoAlterno(TComando comando) => EsFlujoAlternoPorDefecto;
    }
}
