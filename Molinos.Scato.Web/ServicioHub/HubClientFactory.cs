using Ninject;
using Ninject.Parameters;

namespace Molinos.Scato.Web.ServicioHub
{
    public class HubClientFactory
    {
        private readonly IKernel kernel;

        public HubClientFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public HubClient GetClient(string hubName)
        {
            return kernel.Get<HubClient>(new ConstructorArgument("hubName", hubName));
        }

        public HubClientNotificar GetClientNotificar(string hubName)
        {
            return kernel.Get<HubClientNotificar>(new ConstructorArgument("hubName", hubName));
        }
    }
}