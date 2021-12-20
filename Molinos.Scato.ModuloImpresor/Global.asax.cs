using System.Web;
using log4net;
using Molinos.Scato.ModuloImpresor.App_Start;

namespace Molinos.Scato.ModuloImpresor
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            Log4NetConfig.Configure(Server);
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            var logger = LogManager.GetLogger(GetType());
            logger.Error("Excepción no manejada: ", ex);
        }
    }
}