using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarCorrespondeControlDeBalanzaTest
    {
        private VerificarCorrespondeControlDeBalanza target;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarCorrespondeControlDeBalanza();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestControlBalanza()
        {
            srvRepositorio.Setup(s => s.VerificarCorrespondeControlDeBalanza(It.IsAny<Guid>())).Returns(true);
            var resultado = host.TestActivity();
            //var result = ((Resultado)resultado.First(f => f.Key == "Result").Value).Errores.First().Value;
            Assert.That(resultado, Is.Not.Null);
            //Assert.That(result, Is.EqualTo(Textos.ControlDeBalanza_Esperando));
            Assert.That(host.OutArguments.CorrespondeControlDeBalanza, Is.EqualTo(true));
        }
        [Test]
        public void TestSinControlBalanza()
        {
            srvRepositorio.Setup(s => s.VerificarCorrespondeControlDeBalanza(It.IsAny<Guid>())).Returns(false);
            var resultado = host.TestActivity();
            var result = ((Resultado)resultado.First(f => f.Key == "Result").Value).HayErrores;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(host.OutArguments.CorrespondeControlDeBalanza, Is.EqualTo(false));
        }
    

    }
}
