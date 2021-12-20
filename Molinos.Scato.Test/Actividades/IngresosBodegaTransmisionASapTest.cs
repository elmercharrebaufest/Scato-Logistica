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
    public class IngresosBodegaTransmisionASapTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;
        private Mock<IServicioComandos> servcomandos;
        private WorkflowInvokerTest host;
        private IngresosBodegaTransmisionASap target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servSap = new Mock<ZSDWS_SCATO>();
            servcomandos = new Mock<IServicioComandos>();
            target = new IngresosBodegaTransmisionASap();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servcomandos.Object);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servSap.Object);
            host.InArguments.Request = new IngresosBodegaRequest { IngresosBodega = new IngresosBodega { Bins = "100" } };


            servRepositorio.Setup(s => s.ObtenerRecorridoValoresSapPorGuid(It.IsAny<Guid>())).Returns(new ValoresSapDto { DocumentoInternoSap = "Doc", NumeroDeDocumentoSap = "1234" });
            servSap.Setup(s => s.IngresosBodega(It.IsAny<IngresosBodegaRequest>())).Returns(new IngresosBodegaResponse1
                       {
                           IngresosBodegaResponse =
                               new IngresosBodegaResponse
                                   {
                                       EX_RESULTADO =  new ZMMBALANZA6 { MBLNR = "MB", MSGNR = "Mensaje", TEXT = "TEXT", XBLNR = "XBLR" }
                                   }
                       });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>())).Returns(new Resultado());

        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;
            Assert.NotNull(result);
            Assert.False(((Resultado)resultado).HayErrores);

            var funcionaServicio = (bool)host.OutArguments.FuncionaServicio;

            Assert.That(funcionaServicio, Is.True);
            servSap.Verify(s => s.IngresosBodega(It.IsAny<IngresosBodegaRequest>()), Times.Exactly(1));
            servcomandos.Verify(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>()), Times.Exactly(1));
        }


        [Test]
        public void ExceptionTest()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>())).Throws(new Exception("Error"));

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "Error en el procedimiento del servicio SAP: Error");

            var funcionaServicio = (bool)host.OutArguments.FuncionaServicio;

            Assert.That(funcionaServicio, Is.True);
            servSap.Verify(s => s.IngresosBodega(It.IsAny<IngresosBodegaRequest>()), Times.Exactly(1));
            servcomandos.Verify(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>()), Times.Exactly(1));

        }

        [Test]
        public void ResultadoError()
        {
            servSap.Setup(s => s.IngresosBodega(It.IsAny<IngresosBodegaRequest>()))
                   .Returns(new IngresosBodegaResponse1
                   {
                       IngresosBodegaResponse =
                           new IngresosBodegaResponse
                           {
                               EX_RESULTADO = 
                                   new ZMMBALANZA6 { MSGNR = "Mensaje", TEXT = "TXT", XBLNR = "XBLR"}
                           }
                   });

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "TXT");

            var funcionaServicio = (bool)host.OutArguments.FuncionaServicio;

            Assert.That(funcionaServicio, Is.False);
            servSap.Verify(s => s.IngresosBodega(It.IsAny<IngresosBodegaRequest>()), Times.Exactly(1));
            servcomandos.Verify(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>()), Times.Exactly(0));

        }
    }
}
