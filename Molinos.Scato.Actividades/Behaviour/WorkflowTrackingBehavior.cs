using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Ninject;

namespace Molinos.Scato.Actividades.Behaviour
{
    public class WorkflowTrackingBehavior : IServiceBehavior
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
                //host.WorkflowExtensions.Add(() => new WorkflowTrackingParticipant(
                //    ServiceProvider.Current.Get<IServicioComandos>(), 
                //    ServiceProvider.GetL));
                host.WorkflowExtensions.Add(() => ServiceProvider.Current.Get<WorkflowTrackingParticipant>());
            }
        }



        public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}