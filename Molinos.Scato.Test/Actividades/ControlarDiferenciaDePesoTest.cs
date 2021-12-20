using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ControlarDiferenciaDePesoTest
    {
        private ControlarDiferenciaDePeso target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new ControlarDiferenciaDePeso();
            host = WorkflowInvokerTest.Create(target);
        }

        [Test]
        public void TestControlarDiferenciaDePesoNoRepesar()
        {
            var controlRecorrido = new ControlRecorridoDto {Decision = false};
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Repesar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void TestControlarDiferenciaDePesoSiRepesar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true };
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Repesar").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }
    }
}
