using System;
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
    public class RefrescarOrdenDeDescargaFasonTest
    {
        private Scato.Actividades.Internas.RefrescarOrdenDeDescargaFason target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.RefrescarOrdenDeDescargaFason();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestRefrescar()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerOrdenDeDescargaFasonPorInstanceId(It.IsAny<Guid>())).Returns(new OrdenDeDescargaFasonDto { Id = 1 });

            host.InArguments.WorkflowId = new Guid();

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;
            var orden = resultado.First(f => f.Key == "Orden").Value as OrdenDeDescargaFasonDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(orden.Id, Is.EqualTo(1));
        }
    }
}
