using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class AsignarEstablecimientoTest
    {
        private AsignarEstablecimiento target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new AsignarEstablecimiento();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void AsignarEstablecimiento()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ValidarStockEstablecimiento(It.IsAny<int>(), It.IsAny<Guid>())).Returns(new Resultado());
            host.InArguments.EstablecimientoId = 2;
            host.InArguments.InstanceId = guid;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<ModificarRecorridoEstablecimiento>(i => i.InstanceId == guid && i.EstablecimientoId == 2)), Times.Exactly(1));
            srvRepositorio.Verify(p => p.ValidarStockEstablecimiento(It.IsAny<int>(), It.IsAny<Guid>()), Times.Exactly(1));

        }

        [Test]
        public void AsignarEstablecimiento2()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            var result = new Resultado();
            result.Error("","Error 1");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ValidarStockEstablecimiento(It.IsAny<int>(), It.IsAny<Guid>())).Returns(result);
            host.InArguments.EstablecimientoId = 2;
            host.InArguments.InstanceId = guid;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<ModificarRecorridoEstablecimiento>(i => i.InstanceId == guid && i.EstablecimientoId == 2)), Times.Exactly(0));
            srvRepositorio.Verify(p => p.ValidarStockEstablecimiento(It.IsAny<int>(), It.IsAny<Guid>()), Times.Exactly(1));

        }
    }
}
