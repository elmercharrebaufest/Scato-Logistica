using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Test.Mock
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request.RequestUri.AbsoluteUri, request.Method, request));
        }
        public virtual HttpResponseMessage Send(string url, HttpMethod method, HttpRequestMessage request)
        {
            throw new NotImplementedException("Falta Mockear una request a HttpClient - no se deberia pasar por acá porque lo mockeamos");
        }
    }
}
