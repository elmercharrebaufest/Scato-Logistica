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
    public class EgresosMaterialNoProductivoTransmisionASapTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;
        private Mock<IServicioComandos> servcomandos;
        private WorkflowInvokerTest host;
        private EgresosMaterialNoProductivoTransmisionASap target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servSap = new Mock<ZSDWS_SCATO>();
            servcomandos = new Mock<IServicioComandos>();
            target = new EgresosMaterialNoProductivoTransmisionASap();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servcomandos.Object);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servSap.Object);
            host.InArguments.Request = new EgresosNoProductivosRequest { EgresosNoProductivos = new EgresosNoProductivos { Cliente = "A" } };
            

            servRepositorio.Setup(s => s.ObtenerRecorridoValoresSapPorGuid(It.IsAny<Guid>()))
                           .Returns(new ValoresSapDto{DocumentoInternoSap = "Doc", NumeroDeDocumentoSap = "1234"});
            servSap.Setup(s => s.EgresosNoProductivos(It.IsAny<EgresosNoProductivosRequest>()))
                   .Returns(new EgresosNoProductivosResponse1
                       {
                           EgresosNoProductivosResponse =
                               new EgresosNoProductivosResponse
                                   {
                                       Resultado =
                                           new ZMMBALANZA6 {MBLNR = "MB", MSGNR = "Mensaje", TEXT = "TEXT", XBLNR = "XBLR"}
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

        }

        [Test]
        public void ResultadoError()
        {
            servSap.Setup(s => s.EgresosNoProductivos(It.IsAny<EgresosNoProductivosRequest>()))
                   .Returns(new EgresosNoProductivosResponse1
                   {
                       EgresosNoProductivosResponse =
                           new EgresosNoProductivosResponse
                           {
                               Resultado =
                                   new ZMMBALANZA6 {MSGNR = "Mensaje", TEXT = "TXT", XBLNR = "XBLR" }
                           }
                   });

            var result = host.TestActivity();
            var resultado = result.First(s => s.Key == "Result").Value;

            Assert.NotNull(result);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "TXT");

        }
    }
}
