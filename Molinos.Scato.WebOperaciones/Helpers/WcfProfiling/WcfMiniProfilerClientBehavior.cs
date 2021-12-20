using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Molinos.Scato.WebOperaciones.Helpers.WcfProfiling
{
    internal class WcfMiniProfilerClientBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new WcfMiniProfilerClientInspector(endpoint));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override Type BehaviorType
        {
            get { return GetType(); }
        }

        protected override object CreateBehavior()
        {
            return this;
        }
    }
}
