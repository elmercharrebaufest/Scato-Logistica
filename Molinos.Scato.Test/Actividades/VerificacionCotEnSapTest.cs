using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificacionCotEnSapTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;
        private WorkflowInvokerTest host;
        private VerificacionCotEnSap target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servSap = new Mock<ZSDWS_SCATO>();
            target = new VerificacionCotEnSap();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servSap.Object);
            host.InArguments.InstanceId = Guid.NewGuid();

            servRepositorio.Setup(s => s.ObtenerRecorridoValoresSapPorGuid(It.IsAny<Guid>()))
                           .Returns(new ValoresSapDto {DocumentoInternoSap = "A", NumeroDeDocumentoSap = "1000000000000"});
            servSap.Setup(s => s.ValidacionCOT(It.IsAny<ValidacionCOTRequest>()))
                   .Returns(new ValidacionCOTResponse1
                       {
                           ValidacionCOTResponse = new ValidacionCOTResponse {COTAprobado = "X"}
                       });
        }

        [Test]
        public void CotAprobadoTest()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;
            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);
        }


        [Test]
        public void CotNoAprobadoTest()
        {
            servSap.Setup(s => s.ValidacionCOT(It.IsAny<ValidacionCOTRequest>()))
                   .Returns(new ValidacionCOTResponse1
                   {
                       ValidacionCOTResponse = new ValidacionCOTResponse { COTAprobado = "A" }
                   });

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;
            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Keys.FirstOrDefault(), "WorkflowId");
        }

        [Test]
        public void ExceptionTest()
        {
            servRepositorio.Setup(s => s.ObtenerRecorridoValoresSapPorGuid(It.IsAny<Guid>()))
                           .Throws(new Exception("Error"));

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "Error en el procedimiento del servicio SAP: Error");

        }
    }
}
