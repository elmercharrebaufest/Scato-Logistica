using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EliminarRecorridoTest
    {
        private Scato.Actividades.Internas.EliminarRecorrido target;
        private Mock<IServicioComandos> srvComandosMock;
        private Mock<IServicioRepositorio> srvRepopsitorio;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.EliminarRecorrido();
            srvComandosMock = new Mock<IServicioComandos>();
            srvRepopsitorio = new Mock<IServicioRepositorio>();
        }
        [Test]
        public void TestEliminarRecorrido()
        {
            srvComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarRecorrido>())).Returns(new Resultado());
            srvRepopsitorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto());
            target = new Scato.Actividades.Internas.EliminarRecorrido();
            var invoker = new WorkflowInvoker(target);
            invoker.Extensions.Add(() => srvComandosMock.Object);
            invoker.Extensions.Add(() => srvRepopsitorio.Object);
            var resultado = invoker.Invoke();
            var result = resultado.First(f => f.Key == "Result").Value as Resultado;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            srvComandosMock.Verify(v => v.Ejecutar(It.IsAny<EliminarRecorrido>()), Times.Once());
        }
    }
}
