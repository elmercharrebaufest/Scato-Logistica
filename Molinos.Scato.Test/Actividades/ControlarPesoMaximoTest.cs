using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ControlarPesoMaximoTest
    {
        private ControlarPesoMaximo target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomandos;

        [SetUp]
        public void SetUp()
        {
            servcomandos = new Mock<IServicioComandos>();
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoBalanzaBruto>())).Returns(new Resultado());
            target = new ControlarPesoMaximo();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servcomandos.Object);
        }

        [Test]
        public void TestControlarPesoMaximoNoRepesar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = false };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var excedePesoMaximo = resultado.First(f => f.Key == "ExcedePesoMaximo").Value;
            var result = resultado.First(f => f.Key == "Repesar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(excedePesoMaximo, Is.EqualTo(true));
            servcomandos.Verify(s => s.Ejecutar(It.IsAny<ModificarRecorridoBalanzaBruto>()), Times.Never());
        }

        [Test]
        public void TestControlarDiferenciaDePesoSiRepesar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var excedePesoMaximo = resultado.First(f => f.Key == "ExcedePesoMaximo").Value;
            var result = resultado.First(f => f.Key == "Repesar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
            Assert.That(excedePesoMaximo, Is.EqualTo(true));
            servcomandos.Verify(s => s.Ejecutar(It.IsAny<ModificarRecorridoBalanzaBruto>()), Times.Once());
        }
    }
}
