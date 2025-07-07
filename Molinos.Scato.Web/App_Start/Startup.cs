using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Molinos.Scato.Web.App_Start;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(Startup))]
namespace Molinos.Scato.Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ScatoDb"].ConnectionString;
            GlobalHost.DependencyResolver.UseSqlServer(connectionString);
            app.MapSignalR();
        }
    }
}