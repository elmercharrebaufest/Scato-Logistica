using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosPorCompraDeGranosTransmisionASapTest
    {
        private IngresosPorCompraDeGranosTransmisionASap target;
        private WorkflowInvokerTest host;
        private Mock<ZSDWS_SCATO> servSap;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<ZSDWS_SCATO>();
            target = new IngresosPorCompraDeGranosTransmisionASap();
            host = WorkflowInvokerTest.Create(target);

            host.InArguments.Request = new Fill_Z1000Request();

            var resultado = new ZSDES0040[1];
            resultado[0] = new ZSDES0040{MSGNR = "000"};
            servSap.Setup(s => s.Fill_Z1000(It.IsAny<Fill_Z1000Request>())).Returns(new Fill_Z1000Response1(new Fill_Z1000Response{Resultado = resultado}));

            host.Extensions.Add(servSap.Object);
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object resultado;
            object funcionServicio;

            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.False(((Resultado)resultado).HayErrores);

            result.TryGetValue("FuncionaServicio", out funcionServicio);
            Assert.NotNull(funcionServicio);
            Assert.True((bool)funcionServicio);

        }

        [Test]
        public void ExecuteErrorMnsgr()
        {
            var resultado = new ZSDES0040[1];
            resultado[0] = new ZSDES0040 {MSGNR = "1", TEXT = "TXT"};
            servSap.Setup(s => s.Fill_Z1000(It.IsAny<Fill_Z1000Request>()))
                   .Returns(new Fill_Z1000Response1(new Fill_Z1000Response {Resultado = resultado}));

            var result = host.TestActivity();
            Assert.NotNull(result);

            object resultadoExecute;
            result.TryGetValue("Result", out resultadoExecute);
            Assert.NotNull(resultadoExecute);
            Assert.True(((Resultado)resultadoExecute).HayErrores);
            Assert.AreEqual(((Resultado)resultadoExecute).Errores.First(s => s.Key == "InstanciaWorkflow").Value, "TXT");

            object funcionServ;
            result.TryGetValue("FuncionaServicio", out funcionServ);
            Assert.NotNull(funcionServ);
            Assert.False((bool)funcionServ);
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.Fill_Z1000(It.IsAny<Fill_Z1000Request>())).Throws(new Exception("Error"));

            var result = host.TestActivity();
            Assert.NotNull(result);

            object resultado;
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado) resultado).Errores.First(s => s.Key == "InstanciaWorkflow").Value,
                                                                  "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
