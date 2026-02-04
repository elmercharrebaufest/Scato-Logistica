using Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(Molinos.Scato.WebMobile.Startup))]

namespace Molinos.Scato.WebMobile
{
    /// <summary>
    /// OWIN Startup class for configuring authentication middleware
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configuration entry point for OWIN pipeline
        /// </summary>
        /// <param name="app">The OWIN application builder</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
