using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Procesamiento;
using Moq;
using NUnit.Framework;
using Ninject.Extensions.Logging;
using Newtonsoft.Json;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Dominio.Enums;
using Moq.Protected;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorConsultarEscalablesTest
    {
        private ProcesadorConsultarEscalables target;
        private Mock<IRepositorio> mockRepositorio;
        private Mock<IConversor> mockConversor;
        private Mock<ILogger> mockLogger;
        private Mock<IServicioComandos> mockServicioComandos;
        private Mock<HttpMessageHandler> mockMessageHandler;

        [SetUp]
        public void SetUp()
        {
            mockRepositorio = new Mock<IRepositorio>();
            mockConversor = new Mock<IConversor>();
            mockLogger = new Mock<ILogger>();
            mockMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(mockMessageHandler.Object);
            target = new ProcesadorConsultarEscalables(mockRepositorio.Object, mockConversor.Object, mockLogger.Object, httpClient, mockServicioComandos.Object);
        }


        [Test]
        public void ServicioCNRT_ConPatenteNULL()
        {
            var comando = new ConsultarEscalables { Patente = null };

            var resultado = target.Ejecutar(comando);

            Assert.IsTrue(resultado.HayErrores);
        }

        [Test]
        public void ServicioCNRT_ConPatenteCorrecta()
        {
            var comando = new ConsultarEscalables { Patente = "ABC123" };

            var httpClient = new HttpClient(new MockHttpMessageHandler());
           

            var servicio = new ProcesadorConsultarEscalables(mockRepositorio.Object, mockConversor.Object, mockLogger.Object, httpClient, mockServicioComandos.Object);

            var resultado = servicio.Ejecutar(comando) as ResultadoEscalables;
            
            Assert.IsFalse(resultado.HayErrores);
            Assert.AreEqual(TipoVehiculo.Camión, resultado.Categoria);
        }

        [Test]
        public void ServicioCNRTFalla_ConErrores()
        {
            var comando = new ConsultarEscalables { Patente = "ABC123" };

            var resultado = target.Ejecutar(comando);

            Assert.IsTrue(resultado.HayErrores);
           
        }

    }


    public interface IHttpClientWrapper
    {
        HttpResponseMessage GetAsync(string requestUri);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public  HttpResponseMessage GetAsync(string requestUri)
        {
            return  _client.GetAsync(requestUri).Result;
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsoluteUri.Contains("api.cnrt.gob.ar"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"result\": \"ok\", \"status\": 200, \"data\": {\"categoriaEscalado\": null, \"pbtc\": null, \"dominios\": [{\"dominio\": \"UOY455\" , \"rto\": {\"cantEjes\": 2,}}] },\"userMessage\": null,\"actions\": null}")
                };
            }
            else if (request.RequestUri.AbsoluteUri.Contains("error"))
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"error\":\"Error\"}")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"error\":\"Unknown URL\"}")
            };
        }
    }


}
