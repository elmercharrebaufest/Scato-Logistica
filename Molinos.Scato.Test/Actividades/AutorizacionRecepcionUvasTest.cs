using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class AutorizacionRecepcionUvasTest
    {
        private AutorizacionRecepcionUvas target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new AutorizacionRecepcionUvas();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestControlRecorridoNoRechazar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = false };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Reintentar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void TestControlRecorridoInvalidoSiRechazar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Reintentar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }
    }
}
