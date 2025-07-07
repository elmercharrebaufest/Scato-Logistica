using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Actividades.Behaviour
{
    /// <summary>
    ///     Behaviour custom para acceder a los servicios de Scato desde el workflow. No se pueden injectar por Ninject en un ServiceHost.
    /// </summary>
    public class ServiciosScatoBehavior : IServiceBehavior
    {
        public virtual void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                                 Collection<ServiceEndpoint> endpoints,
                                                 BindingParameterCollection bindingParameters)
        {
        }

        public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var host = serviceHostBase as WorkflowServiceHost;
            if (host != null)
            {
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<IServicioRepositorio>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<IServicioComandos>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<IServicioNotificarUsuario>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<IFirmaProvider>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<IServicioSapAsincronico>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<ZSDWS_SCATO>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<WaybillManagementPODv2>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<DatosPort>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<Scato.Servicios.ComplianceWebService.DatosPort>());
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<ILogger>());
            }
        }



        public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}