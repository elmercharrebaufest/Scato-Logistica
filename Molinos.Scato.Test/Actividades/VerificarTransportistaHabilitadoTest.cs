using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarTransportistaHabilitadoTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private WorkflowInvokerTest host;
        private VerificarTransportistaHabilitado target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            target = new VerificarTransportistaHabilitado();
            host = WorkflowInvokerTest.Create(target);

            host.InArguments.Patente = "AAA111";
            host.InArguments.ChoferId = 1;
            host.InArguments.CentroId = 1;

            servRepositorio.Setup(s => s.ChoferInhabilitado(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            servRepositorio.Setup(s => s.CamionInhabilitado(It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            host.Extensions.Add(servRepositorio.Object);
        }

        [Test]
        public void ExecuteHabilitado()
        {
            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True((bool)resultado);
        }

        [Test]
        public void ExecuteDeshabilitado()
        {
            servRepositorio.Setup(s => s.CamionInhabilitado(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.False((bool)resultado);
        }
    }
}
