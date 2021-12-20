using System.ServiceModel;
using Ninject;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioRepositorioFactory : IServicioRepositorioFactory
    {
        private readonly IKernel kernel;

        public ServicioRepositorioFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IServicioRepositorio CrearServicio(string url)
        {
            //TODO: ver como hacer el close del channel, del factory se encarga Ninject
            return kernel.Get<ChannelFactory<IServicioRepositorio>>().CreateChannel(new EndpointAddress(url + "ServicioRepositorio.svc?wsdl"));
        }
    }

}
