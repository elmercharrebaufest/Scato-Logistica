using System.Web;
using Molinos.Scato.ServiciosWeb.App_Start;
using log4net;

namespace Molinos.Scato.ServiciosWeb
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