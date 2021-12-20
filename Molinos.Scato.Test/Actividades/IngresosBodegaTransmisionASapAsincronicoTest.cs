using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosBodegaTransmisionASapAsincronicoTest
    {
        private IngresosBodegaTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioSapAsincronico> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            servSap = new Mock<IServicioSapAsincronico>();
            target = new IngresosBodegaTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio);
            host.Extensions.Add(servcomando);
            host.Extensions.Add(servSap.Object);
        }

        [Test]
        public void TestEjecutar()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new IngresosBodegaAsincronicoDto {  IngresosBodega = new IngresosBodegaRequest { IngresosBodega = new IngresosBodega { Bins = "100" } } };

            var result = host.TestActivity();

            var funcionaServicio = (bool)host.OutArguments.FuncionaServicio;

            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 0);
            Assert.That(funcionaServicio, Is.True);
            servSap.Verify(s => s.IngresosBodega(It.IsAny<Guid>(), It.IsAny<IngresosBodegaAsincronicoDto>()), Times.Exactly(1));
        }

        [Test]
        public void TestEjecutarException()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new IngresosBodegaAsincronicoDto { IngresosBodega = new IngresosBodegaRequest { IngresosBodega = new IngresosBodega { Bins = "100" } } };
            servSap.Setup(s => s.IngresosBodega(It.IsAny<Guid>(), It.IsAny<IngresosBodegaAsincronicoDto>())).Throws(new Exception("Error"));

            var result = host.TestActivity();

            var funcionaServicio = (bool)host.OutArguments.FuncionaServicio;

            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 1);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.ElementAt(0).Value, "Error en el procedimiento del servicio SAP: Error");
            Assert.That(funcionaServicio,Is.False);
            servSap.Verify(s => s.IngresosBodega(It.IsAny<Guid>(), It.IsAny<IngresosBodegaAsincronicoDto>()), Times.Exactly(1));
        }
    }
}
