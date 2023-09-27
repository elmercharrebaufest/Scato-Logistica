using Hangfire;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Molinos.Scato.Dependencias;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Impl;
using Molinos.Scato.Web.ServicioHub;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Web;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Molinos.Scato.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Molinos.Scato.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Molinos.Scato.Web.App_Start
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
            kernel.Load(new WebNinjectModule());
            kernel.Bind<IFirmwareFactory, FirmwareFactory>().To<FirmwareFactory>().InSingletonScope();
            kernel.Bind<HubClient>().ToSelf().InSingletonScope();
            kernel.Bind<HubClientNotificar>().ToSelf().InSingletonScope();
            kernel.Bind<HubClientFactory>().ToSelf().InSingletonScope();
        }        
    }
}
