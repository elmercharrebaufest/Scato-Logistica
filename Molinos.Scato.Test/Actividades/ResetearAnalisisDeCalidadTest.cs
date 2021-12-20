using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ResetearAnalisisDeCalidadTest
    {
        private ResetearAnalisisDeCalidad target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new ResetearAnalisisDeCalidad();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestResetearAnalisisDeCalidad()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoAnalisisDeCalidad>())).Returns(new Resultado());
            host.TestActivity();
            srvComandos.Verify(v => v.Ejecutar(It.Is<ModificarRecorridoAnalisisDeCalidad>(f => true)), Times.Once());
        }
    }
}
