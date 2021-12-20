using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class PuestoComandoTest
    {
        private Scato.Actividades.Internas.PuestoComando target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.PuestoComando();
            host = WorkflowInvokerTest.Create(target);
        }

        [Test]
        public void TestAsignado()
        {
            host.InArguments.ControlRecorrido = new ControlRecorridoDto(){NombreUsuario = "usuario"};
            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "NombreUsuario").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo("usuario"));
        }
    }
}
