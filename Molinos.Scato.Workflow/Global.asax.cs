using System.IO;
using System.ServiceModel;
using System.Web;
using System.Web.Hosting;
using Molinos.Scato.Servicios;
using Molinos.Scato.Workflow.VirtualPath;
using log4net;

namespace Molinos.Scato.Workflow
{
    public class Global : HttpApplication
    {

        protected void Application_Start()
        {
            var fileInfo = new FileInfo(Server.MapPath("~/log4net.config"));
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);

            var servicio = new ChannelFactory<IServicioWorkflows>("ServicioWorkflows").CreateChannel();
            HostingEnvironment.RegisterVirtualPathProvider(new WorkflowVirtualPathProvider(servicio));
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            var logger = LogManager.GetLogger(GetType());
            logger.Error("Excepción no manejada: ", ex);
        }
    }
}