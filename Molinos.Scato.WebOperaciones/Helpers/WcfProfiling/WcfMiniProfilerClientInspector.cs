using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using StackExchange.Profiling;

namespace Molinos.Scato.WebOperaciones.Helpers.WcfProfiling
{
    public class WcfMiniProfilerClientInspector : IClientMessageInspector
    {
        private readonly ServiceEndpoint endpoint;

        public WcfMiniProfilerClientInspector(ServiceEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return MiniProfiler.Current.Step("Web Service: " + request.Headers.Action);
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var step = correlationState as IDisposable;
            if(step != null)
            {
                step.Dispose();
            }
        }
    }
}
