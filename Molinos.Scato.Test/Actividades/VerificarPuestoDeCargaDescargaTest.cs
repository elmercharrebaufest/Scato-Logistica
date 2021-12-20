using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarPuestoDeCargaDescargaTest
    {
        private VerificarPuestoDeCargaDescarga target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarPuestoDeCargaDescarga();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void VerificarPuestoCorrecto()
        {
            var instanceId = new Guid();
            const int puesto = 10;
            srvRepositorio.Setup(s => s.VerificarPuestodeCargaDescarga(instanceId, puesto)).Returns(true);
            
            host.InArguments.InstanceId = instanceId;
            host.InArguments.PuestoDeTrabajoId = 10;
            host.TestActivity();
            var resultado = (Resultado) host.OutArguments.Result;
            var puestoCorrecto = (bool) host.OutArguments.PuestoCorrecto;

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(puestoCorrecto, Is.True);
        }

        [Test]
        public void VerificarPuestoIncorrecto()
        {
            var instanceId = new Guid();
            const int puesto = 10;
            srvRepositorio.Setup(s => s.VerificarPuestodeCargaDescarga(instanceId, puesto)).Returns(false);

            host.InArguments.InstanceId = instanceId;
            host.InArguments.PuestoDeTrabajoId = 10;
            host.TestActivity();
            var resultado = (Resultado)host.OutArguments.Result;
            var puestoCorrecto = (bool)host.OutArguments.PuestoCorrecto;

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(puestoCorrecto, Is.False);
        }
    }
}
