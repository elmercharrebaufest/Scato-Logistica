using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using RechazarCamion = Molinos.Scato.Actividades.Internas.RechazarCamion;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RechazarCamionTest
    {
        private RechazarCamion target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new RechazarCamion();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void RechazarCamion()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.RechazarCamion>())).Returns(new Resultado());
            host.InArguments.WorkflowInstanceId = guid;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<Dominio.Comandos.RechazarCamion>(i => i.WorkflowId == guid)), Times.Exactly(1));

        }
    }
}
