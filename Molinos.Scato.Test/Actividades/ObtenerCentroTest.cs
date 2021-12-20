using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ObtenerCentroTest
    {
        private ObtenerCentro target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<ScatoPersistenceParticipant> srvscato;
        [SetUp]
        public void SetUp()
        {
            target = new ObtenerCentro();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvscato = new Mock<ScatoPersistenceParticipant>();
            host.Extensions.Add(srvscato.Object);
            
        }

        [Test]
        public void TestObtenerCentro()
        {
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto{Id = 2});
            srvscato.Object.CentroId = 2;
            var resultado = host.TestActivity();

            var centro = resultado.First(f => f.Key == "Result").Value as CentroDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(centro.Id, Is.EqualTo(2));

        }

        [Test]
        public void TestObtenerCentroError()
        {
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 2 });
            srvscato.Object.CentroId = null;
            var resultado = host.TestActivity();

            var centro = resultado.First(f => f.Key == "Result").Value as CentroDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(centro, Is.Null);

        }
    }
}
