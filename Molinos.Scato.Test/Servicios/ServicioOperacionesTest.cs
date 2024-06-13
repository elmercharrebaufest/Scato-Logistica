using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Moq;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

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
        private string jsonErrorResponseJsonEntity;
        private string jsonErrorResponseJsonSimple;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["APITokenOperacionesAPI"] = "test-token";

            mockRestResponse = new Mock<IRestResponse>();
            mockRestRequest = new Mock<IRestRequest>();

            mockRestClient = new Mock<IRestClient>();

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

            var errorResponseJsonSimple = new Dictionary<string, string>
            {
                ["Message"] = "\"Authorization has been denied for this request.\"",
            };

            var errorResponseJsonEntity = new ErrorResponse
            {
                Message = "An error occurred",
                ExceptionMessage = "Exception message",
                ExceptionType = "Exception type",
                StackTrace = "Stack trace",
                InnerException = new InnerException
                {
                    Message = "Inner exception message",
                    ExceptionMessage = "Inner exception message",
                    ExceptionType = "Inner exception type",
                    StackTrace = "Inner stack trace"
                }
            };

            this.jsonErrorResponseJsonEntity = JsonConvert.SerializeObject(errorResponseJsonEntity);
            this.jsonErrorResponseJsonSimple = JsonConvert.SerializeObject(errorResponseJsonSimple);

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
        public void InformarViajeOrdenesDeCargaFason_EnvioCorrecto_DebeLoguearExito()
        {
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
            Assert.AreEqual("El valor no puede ser nulo.\r\nNombre del parámetro: value", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ConUnaRespuestaJsonSimple()
        {
            var mockRestResponse = new Mock<IRestResponse>();
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);
            mockRestResponse.Setup(response => response.Content).Returns(jsonErrorResponseJsonSimple);
            mockRestResponse.Setup(response => response.StatusCode).Returns(HttpStatusCode.InternalServerError);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("An error occurred: Exception message\nInner exception: Inner exception message"));

            var ex = Assert.Throws<Exception>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("An error occurred: Exception message\nInner exception: Inner exception message", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ConUnaRespuestaJsonComplejo()  
        {
            var mockRestResponse = new Mock<IRestResponse>();
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);
            mockRestResponse.Setup(response => response.Content).Returns(jsonErrorResponseJsonEntity);
            mockRestResponse.Setup(response => response.StatusCode).Returns(HttpStatusCode.InternalServerError);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("An error occurred: Exception message\nInner exception: Inner exception message"));

            var ex = Assert.Throws<Exception>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("An error occurred: Exception message\nInner exception: Inner exception message", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ConUnaRespuesta_BadRequest()
        {
            var mockRestResponse = new Mock<IRestResponse>();
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);
            mockRestResponse.Setup(response => response.Content).Returns(jsonErrorResponseJsonEntity);
            mockRestResponse.Setup(response => response.StatusCode).Returns(HttpStatusCode.BadRequest);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("Parámetros de solicitud incorrectos. Verifique los parámetros enviados a MoaOperaciones"));

            var ex = Assert.Throws<Exception>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("Parámetros de solicitud incorrectos. Verifique los parámetros enviados a MoaOperaciones", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ConUnaRespuesta_Forbidden()
        {
            var mockRestResponse = new Mock<IRestResponse>();
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);
            mockRestResponse.Setup(response => response.Content).Returns(jsonErrorResponseJsonEntity);
            mockRestResponse.Setup(response => response.StatusCode).Returns(HttpStatusCode.Forbidden);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("Error de autenticación. Token inválido o falta de permisos en MoaOperaciones"));

            var ex = Assert.Throws<Exception>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("Error de autenticación. Token inválido o falta de permisos en MoaOperaciones", ex.Message);
        }

        [Test]
        public void InformarViajeOrdenesDeCargaFason_ErrorEnElServicio_ConUnaRespuesta_NotFound()
        {
            var mockRestResponse = new Mock<IRestResponse>();
            mockRestResponse.Setup(response => response.IsSuccessful).Returns(false);
            mockRestResponse.Setup(response => response.ErrorException);
            mockRestResponse.Setup(response => response.Content).Returns(jsonErrorResponseJsonEntity);
            mockRestResponse.Setup(response => response.StatusCode).Returns(HttpStatusCode.NotFound);

            mockRestClient.Setup(client => client.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(mockRestResponse.Object);

            mockExternalServiceException.Setup(e => e.ThrowException(It.IsAny<string>())).Throws(new Exception("El endpoint de MoaOperaciones especificado no fue encontrado o el servicio no responde. Verifique la URL del servicio."));

            var ex = Assert.Throws<Exception>(() => servicioOperaciones.InformarViajeOrdenesDeCargaFason(ingresosEgresosFasonesDto));
            Assert.AreEqual("El endpoint de MoaOperaciones especificado no fue encontrado o el servicio no responde. Verifique la URL del servicio.", ex.Message);
        }
    }


}

