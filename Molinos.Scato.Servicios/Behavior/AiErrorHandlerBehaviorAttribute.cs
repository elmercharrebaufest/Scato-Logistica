using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Collections.ObjectModel;
using Microsoft.ApplicationInsights;

namespace Molinos.Scato.Servicios.Behavior
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AiErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior, IErrorHandler
    {
        protected Type ServiceType { get; set; }
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            ServiceType = serviceDescription.ServiceType;
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(this);
            }
        }

        public bool HandleError(Exception error)
        {
            var ai = new TelemetryClient();
            ai.TrackException(error);

            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
