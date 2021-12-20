using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ComandosEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorFinDeWorkflowTest
    {
        private ProcesadorFinDeWorkflow target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorFinDeWorkflow(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEjecutarOK()
        {
            var guid = new Guid();
            var resultado = target.Ejecutar(new FinDeWorkflow {InstanceId = guid});
            repositorioMock.Verify(r => r.EjecutarComando(It.IsAny<ArchivarLogActividad>()), Times.Once());
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void TestEjecutarError()
        {
            var guid = new Guid();
            repositorioMock.Setup(r => r.EjecutarComando(It.IsAny<ArchivarLogActividad>())).Throws<Exception>();
            var resultado = target.Ejecutar(new FinDeWorkflow {InstanceId = guid});
            repositorioMock.Verify(r => r.EjecutarComando(It.IsAny<ArchivarLogActividad>()), Times.Once());
            Assert.That(resultado.HayErrores, Is.True);
        }
    }
 
}
