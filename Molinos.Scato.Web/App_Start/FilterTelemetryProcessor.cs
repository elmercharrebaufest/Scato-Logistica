using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;

namespace Molinos.Scato.Web.App_Start
{
    public class FilterTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public FilterTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            var request = item as RequestTelemetry;

            if (request != null && request.ResponseCode.Equals("401", StringComparison.OrdinalIgnoreCase))
            {
                // To filter out an item, just terminate the chain: 
                return;
            }

            var dependencyRequest = item as DependencyTelemetry;
            if (dependencyRequest != null &&
                (dependencyRequest.Success == false &&
                dependencyRequest.Type == "HTTP" &&
                dependencyRequest.Target == "localhost:8080" &&
                dependencyRequest.Name == "POST /ServicioOrquestador" &&
                dependencyRequest.ResultCode == "401"))
            {
                return;
            }


            this.Next.Process(item);
        }
    }


}