using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class InicializarCaladoTest
    {
        private InicializarCalado target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new InicializarCalado();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void InicializarCalado()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Empty);

        }
    }
}
