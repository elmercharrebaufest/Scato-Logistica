using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using AlmacenarPrecintos = Molinos.Scato.Actividades.Internas.AlmacenarPrecintos;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class AlmacenarPrecintosTest
    {
        private AlmacenarPrecintos target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new AlmacenarPrecintos();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestAlmacenarPrecintos()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.Precintos = new[] {new PrecintoDto{NumeroPrecinto = "1", Detalle = "Detalle 1"}};

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestAlmacenarPrecintosInvalidos()
        {
            var resultadoTransaccion = new Resultado();
            resultadoTransaccion.Errores.Add("", "Error");
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(resultadoTransaccion);

            host.InArguments.Precintos = new[] { new PrecintoDto { NumeroPrecinto = "1", Detalle = "Detalle 1" } };

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
        }
    }
}
