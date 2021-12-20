using System.ServiceModel;
using Ninject;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioComandosFactory : IServicioComandosFactory
    {
        private readonly IKernel kernel;

        public ServicioComandosFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IServicioComandos CrearServicio(string url)
        {
            //TODO: ver como hacer el close del channel, del factory se encarga Ninject
            return kernel.Get<ChannelFactory<IServicioComandos>>().CreateChannel(new EndpointAddress(url + "ServicioComandos.svc?wsdl"));
        }
    }

}
