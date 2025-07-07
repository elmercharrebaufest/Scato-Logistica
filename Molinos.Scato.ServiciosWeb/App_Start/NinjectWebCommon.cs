using Hangfire;
using log4net;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Molinos.Scato.Dependencias;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Seguridad;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Molinos.Scato.ServiciosWeb.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Molinos.Scato.ServiciosWeb.App_Start.NinjectWebCommon), "Stop")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Molinos.Scato.ServiciosWeb.App_Start.NinjectWebCommon), "PostStart")]

namespace Molinos.Scato.ServiciosWeb.App_Start
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

        public static void PostStart()
        {
            Task.Run(() =>
                {
                    try
                    {
                        Thread.Sleep(1000);
                        bootstrapper.Kernel.Get<IServicioComandos>().Ejecutar(new SuscribirEventos());
                    }
                    catch (Exception e)
                    {
                        var logger = LogManager.GetLogger(typeof (NinjectWebCommon));
                        logger.Error("Ocurrio un error al suscribir eventos.", e);
                    }
                });
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
            
            RegisterServices(kernel);
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new ServiciosWebNinjectModule());
            AuthorizationManager.KernelInstance = kernel;
        }        
    }
}
