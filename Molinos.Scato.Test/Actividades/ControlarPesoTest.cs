using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ControlarPesoTest
    {
        private ControlarPeso target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new ControlarPeso();
            host = WorkflowInvokerTest.Create(target);
        }

        [Test]
        public void TestControlarPesoOrigenAprobar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Rechazado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void TestControlarDiferenciaDePesoRechazar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = false };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Rechazado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }
    }
}
