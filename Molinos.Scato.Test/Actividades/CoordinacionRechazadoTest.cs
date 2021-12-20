using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CoordinacionRechazadoTest
    {
        private CoordinacionRechazado target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new CoordinacionRechazado();
            host = WorkflowInvokerTest.Create(target);
        }

        [Test]
        public void TestCoordinacionRechazadoOrigenAprobar()
        {
            var controlRecorrido = new ControlRecorridoDto { Decision = true, Mensaje = "a", Comentario = "b"};
            host.InArguments.ControlRecorrido = controlRecorrido;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Observacion").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo("a\nb"));
        }

    }
}
