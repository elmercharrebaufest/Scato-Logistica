using System.ServiceModel;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioImpresorFactory : IServicioImpresorFactory
    {
        private readonly IKernel kernel;

        public ServicioImpresorFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IServicioImpresion CrearServicio()
        {
            return kernel.Get<ChannelFactory<IServicioImpresion>>().CreateChannel();
        }
    }

}
