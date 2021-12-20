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
    public class AutorizarTiempoEnTransitoTest
    {
        private Molinos.Scato.Actividades.Internas.AutorizarTiempoEnTransito target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Molinos.Scato.Actividades.Internas.AutorizarTiempoEnTransito();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestAutorizarTiempoEnTransito()
        {
            var autorizarTiempo = new AutorizacionTiempoEnTransitoDto();

            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.AutorizacionTiempoDto = autorizarTiempo;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestAutorizarTiempoEnTransitoInvalido()
        {
            var autorizarTiempo = new AutorizacionTiempoEnTransitoDto();
            var resultadoTransaccion = new Resultado();
            resultadoTransaccion.Errores.Add("", "Error");
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(resultadoTransaccion);

            host.InArguments.AutorizacionTiempoDto = autorizarTiempo;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
        }
    }
}
