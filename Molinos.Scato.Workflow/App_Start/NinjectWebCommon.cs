using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dependencias;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using Ninject.Web.Common;
using System;
using System.Linq;
using System.Web;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Molinos.Scato.Workflow.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Molinos.Scato.Workflow.App_Start.NinjectWebCommon), "Stop")]

namespace Molinos.Scato.Workflow.App_Start
{
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
           
            //RegisterLogger(kernel);
            RegisterServices(kernel);
            
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //Agrego el binding aca porque sino genera dependencias circulares
            kernel.Bind<IListaDeWorkflows>().To<ListaDeWorkflows>().InRequestScope();
            kernel.Load(new WorkflowNinjectModule());
            var bindings = kernel.GetBindings(typeof(ILogger)).ToList();

            // Eliminar bindings previos
            foreach (var binding in bindings)
            {
                kernel.RemoveBinding(binding);
            }
            kernel.Bind<ILogger>().ToMethod(ctx =>
            {
                var factory = new Log4NetLoggerFactory();
                return factory.GetLogger(typeof(ILogger));
            }).InSingletonScope();
            ServiceProvider.Current = kernel;    
        }

        private static void RegisterLogger(IKernel kernel)
        {
                var bindings = kernel.GetBindings(typeof(ILogger)).ToList();

            // Eliminar bindings previos
            foreach (var binding in bindings)
            {
                kernel.RemoveBinding(binding);
            }
            kernel.Bind<ILogger>().ToMethod(ctx =>
                {
                    var factory = new Log4NetLoggerFactory();
                    return factory.GetLogger(typeof(ILogger));
                }).InSingletonScope();

        }

    }
}
