using System;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.ServiceModel;
using System.Web;
using Molinos.Scato.Servicios;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WfEditor.ViewModel
{
    public class ViewModelLocator : IDisposable
    {      
        private readonly IKernel kernel;

        public ViewModelLocator()
        {
            var commandLineArgs = GetArguments();
            var ambiente = string.Empty;
            if (commandLineArgs.Length > 1)
            {
                ambiente = commandLineArgs[1];
            }

            log4net.GlobalContext.Properties["Ambiente"] = ambiente;
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            kernel = new StandardKernel();
            kernel.Bind<IServicioWorkflows>()
                .ToMethod(context => new ChannelFactory<IServicioWorkflows>("ServicioWorkflows"+ambiente).CreateChannel());

            kernel.Bind<MainWindowViewModel>().ToSelf()
                .WithConstructorArgument("ambiente", ambiente)
                .WithConstructorArgument("dominio", ConfigurationManager.AppSettings["Dominio"+ambiente]);
        }

        public static string[] GetArguments()
        {

            if (ApplicationDeployment.IsNetworkDeployed &&
                ApplicationDeployment.CurrentDeployment.ActivationUri != null)
            {
                string query = HttpUtility.UrlDecode(
                ApplicationDeployment.CurrentDeployment.ActivationUri.Query);

                if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
                {
                    string[] arguments = query.Substring(1).Split(' ');
                    string[] commandLineArgs = new string[arguments.Length + 1];
                    commandLineArgs[0] = Environment.GetCommandLineArgs()[0];
                    arguments.CopyTo(commandLineArgs, 1);
                    return commandLineArgs;
                }
            }
            return Environment.GetCommandLineArgs();

        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return kernel.Get<MainWindowViewModel>(); }
        }

        public AbrirWorkflowViewModel AbrirWorkflowViewModel
        {
            get { return kernel.Get<AbrirWorkflowViewModel>(); }
        }

        public GuardarWorkflowViewModel GuardarWorkflowViewModel
        {
            get { return kernel.Get<GuardarWorkflowViewModel>(); }
        }

        public ActivarWorkflowViewModel ActivarWorkflowViewModel
        {
            get { return kernel.Get<ActivarWorkflowViewModel>(); }
        }

        public ILogger GetLogger(Type type)
        {
            return kernel.Get<ILoggerFactory>().GetLogger(type);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                kernel.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
