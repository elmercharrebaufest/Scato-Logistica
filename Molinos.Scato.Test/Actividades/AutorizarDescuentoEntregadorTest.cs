using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class AutorizarDescuentoEntregadorTest
    {
        private Scato.Actividades.Internas.AutorizarDescuentoEntregador target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.AutorizarDescuentoEntregador();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestAutorizarDescuentoEntregador()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarDecisionEntregadorCalado>())).Returns(new Resultado());

            host.InArguments.Calado = new CaladoDto();
            host.InArguments.ControlRecorrido = new ControlRecorridoDto{ Decision = true};

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "DecisionEntregador").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void TestNoAutorizarDescuentoEntregador()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarDecisionEntregadorCalado>())).Returns(new Resultado());

            host.InArguments.Calado = new CaladoDto();
            host.InArguments.ControlRecorrido = new ControlRecorridoDto { Decision = false };

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "DecisionEntregador").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }
    }
}
