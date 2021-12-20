using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class BalanzaACeroSiEsTrenTest
    {
        private BalanzaACeroSiEsTren target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new BalanzaACeroSiEsTren();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void AsignarTarjeta()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.BalanzaId = 5;
            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(
                p => p.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(cmd => cmd.BalanzaId == 5 && cmd.EstaEnCero == true)),
                Times.Exactly(1));
        }
    }
}
