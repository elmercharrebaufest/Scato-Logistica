using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Imp;
using Moq;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class ServicioHealthCheckTests
    {
        private Mock<ILogger> mockLogger;

        [SetUp]
        public void SetUp()
        {
            mockLogger = new Mock<ILogger>();
        }

        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> responder;

            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                this.responder = responder;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = responder(request);
                return Task.FromResult(response);
            }
        }

        [Test]
        public async Task TestConsultaHealthCheckRetornaConectado()
        {
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK")
            });
            var client = new HttpClient(handler);
            var service = new ServicioHealthCheck(client, mockLogger.Object);
            var external = new MonitoreoServicioExternoDto
            {
                HealthCheckUrl = "http://test/api/health",
                HealthCheckConfig = null
            };

            var result = await service.CheckAsync(external, CancellationToken.None);

            Assert.AreEqual(HealthCheckStatus.Conectado, result.Status);
        }

        [Test]
        public async Task TestConsultaHealthCheckRetornaDesconectado()
        {
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("some content without marker")
            });
            var client = new HttpClient(handler);
            var service = new ServicioHealthCheck(client, mockLogger.Object);
            var cfg = new HealthCheckConfigDto { SuccessContains = "healthy" };
            var external = new MonitoreoServicioExternoDto
            {
                HealthCheckUrl = "http://test/api/checkcontains",
                HealthCheckConfig = Newtonsoft.Json.JsonConvert.SerializeObject(cfg)
            };

            var result = await service.CheckAsync(external, CancellationToken.None);

            Assert.AreEqual(HealthCheckStatus.Desconectado, result.Status);
        }

        [Test]
        public async Task TestConsultaHealthCheckRetornaDesconectadoPorException()
        {
            var handler = new FakeHandler(req => throw new HttpRequestException("request failed"));
            var client = new HttpClient(handler);
            var service = new ServicioHealthCheck(client, mockLogger.Object);
            var external = new MonitoreoServicioExternoDto
            {
                HealthCheckUrl = "http://test/service",
                HealthCheckConfig = null
            };

            var result = await service.CheckAsync(external, CancellationToken.None);

            Assert.AreEqual(HealthCheckStatus.Desconectado, result.Status);
        }
    }
}