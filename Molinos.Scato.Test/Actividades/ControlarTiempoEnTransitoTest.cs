using System.Linq;
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
    public class ControlarTiempoEnTransitoTest
    {
        private ControlarTiempoEnTransito target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> srvComandos;

        [SetUp]
        public void SetUp()
        {
            srvComandos = new Mock<IServicioComandos>();

            target = new ControlarTiempoEnTransito();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);

        }

        [Test]
        public void TestControlarTiempo()
        {
            var control = new ControlRecorridoDto() { Decision = false };
            host.InArguments.ControlRecorrido = control;
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            var resultado = host.TestActivity();

            var autoriza = resultado.First(f => f.Key == "Autoriza").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(autoriza, Is.EqualTo(false));
        }
    }
}
