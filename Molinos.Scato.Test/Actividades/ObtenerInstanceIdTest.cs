using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ObtenerInstanceIdTest
    {
        private WorkflowInvokerTest host;
        private ObtenerInstanceId target;
        
        [SetUp]
        public void SetUp()
        {
            target = new ObtenerInstanceId();
            host = WorkflowInvokerTest.Create(target);
        }

        [Test]
        public void ObtenerInstancia()
        {
            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            Assert.That(resultado.FirstOrDefault().Value, Is.TypeOf(typeof (Guid)));
        }
    }
}
