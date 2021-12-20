using System;
using System.Configuration;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class LlegadaADestinoEnRedespachosGenerarRequestTest
    {
        private LlegadaADestinoEnRedespachosGenerarRequest target;
        private Mock<IServicioComandos> servcomandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new LlegadaADestinoEnRedespachosGenerarRequest();
            servcomandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servcomandos.Object);
            host.InArguments.Documento = "123456789012";
            host.InArguments.Ejercicio = 1;
            host.InArguments.FechaContab = new DateTime(2015,6,6);
            host.InArguments.DocumentoInternoSap = "Doc";

        }

        [Test]
        public void GenerarRequestLogSapTest()
        {

            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
            servcomandos.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));
        }

        [Test]
        public void GenerarRequestSinLogSapTest()
        {
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "0";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
            servcomandos.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Never());
        }

        [Test]
        public void GenerarRequestException()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearControlRecorrido>())).Throws(new Exception("RequestError"));
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "0";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
        }
    }
}
