using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ControlarIngresoTest
    {
        private Scato.Actividades.Internas.ControlarIngreso target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.ControlarIngreso();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestControlarIngreso()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescargaFason>())).Returns(new Resultado());
            srv.Setup(x => x.ObtenerOrdenDeDescargaFason(It.IsAny<int>())).Returns(new OrdenDeDescargaFasonDto{Cliente = "a"});

            host.InArguments.Orden = new OrdenDeDescargaFasonDto { Cliente = "b" };

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "OrdenDeDescargaFason").Value as OrdenDeDescargaFasonDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.Cliente, Is.EqualTo("a"));
        }

        [Test]
        public void TestControlarIngresoError()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarOrdenDeDescargaFason>())).Returns(new Resultado());
            srv.Setup(x => x.ObtenerOrdenDeDescargaFason(It.IsAny<int>())).Returns((OrdenDeDescargaFasonDto)null);

            host.InArguments.Orden = new OrdenDeDescargaFasonDto { Cliente = "b" };

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "OrdenDeDescargaFason").Value as OrdenDeDescargaFasonDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(null));
        }
    }
}
