using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Molinos.Scato.Web.App_Start;
using Owin;

[assembly: OwinStartup(typeof(SignalRStartup))]
namespace Molinos.Scato.Web.App_Start
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ScatoDb"].ConnectionString;
            GlobalHost.DependencyResolver.UseSqlServer(connectionString);
            app.MapSignalR();
        }
    }
}