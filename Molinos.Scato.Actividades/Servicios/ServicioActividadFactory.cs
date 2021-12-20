using System.Configuration;
using Ninject;
using Ninject.Parameters;

namespace Molinos.Scato.Actividades.Servicios
{
    public class ServicioActividadFactory<T> : IServicioActividadFactory<T>//, IDisposable
    {
        private readonly IKernel kernel;

        public ServicioActividadFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public T CrearServicio(int workflowDefinicionId)
        {
            var uri = ConfigurationManager.AppSettings["UrlBaseWorkflow"] + workflowDefinicionId + ".xamlx";
            return kernel.Get<T>(new IParameter[]{new Parameter("uri", uri, true) });
        }
    }
 
}