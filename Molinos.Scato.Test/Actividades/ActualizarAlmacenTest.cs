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
    public class ActualizarAlmacenTest
    {
        private ActualizarAlmacen target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new ActualizarAlmacen();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void AsignarTarjeta()
        {
            var guid = new Guid();
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.AlmacenId = 5;
            host.InArguments.InstanceId = guid;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(
                p => p.Ejecutar(It.Is<ModificarRecorridoAlmacen>(cmd => cmd.AlmacenId == 5 && cmd.InstanceId == guid)),
                Times.Exactly(1));
        }
    }
}
