using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.Procesamiento;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioComandos : IServicioComandos
    {
        private readonly IKernel kernel;
        private readonly ILogger log;
        private IDictionary<Type, Type> procesadores;

        public ServicioComandos(IKernel kernel, ILogger log)
        {
            this.kernel = kernel;
            this.log = log;
            RegistrarProcesadores();
        }

        public Resultado Ejecutar(Comando comando)
        {
            var tipoProcesador = procesadores[comando.GetType()];
            var procesador = (IProcesadorComando) kernel.Get(tipoProcesador);
            return procesador.Ejecutar(comando);
        }

        private void RegistrarProcesadores()
        {
            log.Info("Registrando procesadores de comandos...");
            procesadores = new Dictionary<Type, Type>();

            var comandos = Comando.TiposDeComandos().Where(t => !t.IsAbstract);

            foreach (var comando in comandos)
            {
                var procesador = ObtenerProcesador(comando);
                procesadores.Add(comando, procesador);
                log.Debug("Comando: {0} Procesador: {1}", comando.Name, procesador.Name);
            }
        }

        private Type ObtenerProcesador(Type comando)
        {
            try
            {
                return typeof(IProcesadorComando<>)
                    .Assembly
                    .GetExportedTypes()
                    .Single(
                        x => !x.IsAbstract && x.GetInterfaces().Any(i => i.IsGenericType
                                                        && i.GetGenericTypeDefinition() == typeof(IProcesadorComando<>)
                                                        && i.GetGenericArguments().Single() == comando));
            }
            catch (InvalidOperationException e)
            {
                log.Error(e, "No existe procedador para el comando {0}", comando.Name);
                throw;
            }
        }
        
    }
}
