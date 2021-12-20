using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class DistribuirAlmacenesTest
    {
        private DistribuirAlmacenes target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new DistribuirAlmacenes();
            srvRepositorio = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestDistribuirAlmacenes()
        {
            srvRepositorio.Setup(s => s.Ejecutar(It.IsAny<CrearDistribucionDeAlmacenes>())).Returns(new Resultado());
            host.InArguments.InstanceId = new Guid();
            host.InArguments.DistribucionDeAlmacenes = new DistribucionDeAlmacenesDto();

            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
        }

    }
}
