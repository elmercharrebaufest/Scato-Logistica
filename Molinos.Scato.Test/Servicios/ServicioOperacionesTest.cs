using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebService;
using Molinos.Scato.Servicios.Impl;
using Moq;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using RestSharp;
using System;
using System.Configuration;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    class ServicioOperacionesTest 
    {
        private Mock<ILogger> mockLogger;
        private Mock<IExternalServiceException> mockExternalServiceException;
        private ServicioOperaciones servicioOperaciones;
        private ServicioOperaciones servicioOperacionestest;
        private Mock<IRestClientFactory> mockRestClientFactory;
        private Mock<IRestClient> mockRestClient;
        private IngresosEgresosFasonesDto ingresosEgresosFasonesDto;
        private Mock<IRestResponse> mockRestResponse; 
        private Mock<IRestRequest> mockRestRequest;


        [SetUp]
        public void SetUp()
        {

            ConfigurationManager.AppSettings["URLOperacionesAPI"] = url;

            mockRestResponse = new Mock<IRestResponse>();
            mockRestRequest = new Mock<IRestRequest>();

            mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(client => client.BaseUrl).Returns(new Uri(url));
            mockRestClient.Setup(client => client.Timeout).Returns(5000);

            mockRestClientFactory = new Mock<IRestClientFactory>();
            mockRestClientFactory.Setup(factory => factory.CrearClientOperaciones()).Returns(mockRestClient.Object);

            mockLogger = new Mock<ILogger>();
            mockExternalServiceException = new Mock<IExternalServiceException>();

            servicioOperaciones = new ServicioOperaciones(mockLogger.Object, mockExternalServiceException.Object, mockRestClientFactory.Object);

            ingresosEgresosFasonesDto = new IngresosEgresosFasonesDto
            {
                FasonId = 78596,
                PesadaTara = 45000,
                PesadaNeto = 15000,
                FechaIngreso = "2023-03-20",
                FechaEgreso = "2023-03-21",
                NroRemito = "123456",
                UniMedCant = "Kilogramos"
            };

        }

        [Test]
        public void CrearRequest_SolicitudRestConEncabezadosCorrectos()  
        {
            var recurso = "test/resource";
            var token = "test-token";
            ConfigurationManager.AppSettings["APITokenOperacionesAPI"] = token;

            var request = servicioOperaciones.CrearRequest(recurso);

            Assert.IsNotNull(request);
            Assert.AreEqual(recurso, request.Resource);
            Assert.IsTrue(request.Parameters.Exists(p => p.Name == "X-Api-Key" && (string)p.Value == token));
        }

        [Test]
        public void CrearCliente_LanzarExcepcionSiLaUrlNoEstaConfigurada()   
        {

            mockRestClientFactory.Setup(factory => factory.CrearClientOperaciones()).Throws(new NullReferenceException("La propiedad URLOperacionesAPI no está configurada."));

            var ex = Assert.Throws<NullReferenceException>(() =>
            {
                servicioOperacionestest = new ServicioOperaciones(mockLogger.Object, mockExternalServiceException.Object, mockRestClientFactory.Object);
            });


            Assert.AreEqual("La propiedad URLOperacionesAPI no está configurada.", ex.Message);
        }

        [Test]
        public void CrearRequest_LanzarExcepcionSiElTokenNoEstaConfigurado()   
        {
            ConfigurationManager.AppSettings["APITokenOperacionesAPI"] = null;

            var ex = Assert.Throws<System.NullReferenceException>(() =>
            {
                servicioOperaciones.CrearRequest("test/resource");
            });

            Assert.AreEqual("La propiedad APITokenOperacionesAPI no está configurada.", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_EnvioCorrecto_DebeLoguearExito()
        {
            ConfigurationManager.AppSettings["APITokenOperacionesAPI"] = "test-token";
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(true);
            mockRestResponse.Setup(response => response.StatusCode).Returns(System.Net.HttpStatusCode.OK);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto);

            mockLogger.Verify(log => log.Trace("Viaje informado con éxito."), Times.Once());
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_DebeLanzarExcepcion_ArgumentNullException()
        {
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("Error en el servicio externo."));

            var ex = Assert.Throws<ArgumentNullException>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: value", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ErrorEntity()  
        {
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("Error en el servicio externo."));

            var ex = Assert.Throws<ArgumentNullException>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: value", ex.Message);
        }

    }
}

