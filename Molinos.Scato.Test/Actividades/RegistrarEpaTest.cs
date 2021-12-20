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
    public class RegistrarEpaTest
    {
        private RegistrarEpa target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarEpa();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            host.InArguments.Cosecha = "11-12";
            host.InArguments.InstanceId = new Guid();
            srvRepositorio.Setup(x => x.ObtenerPesoNetoConDescuento(It.IsAny<Guid>())).Returns(24530);
            srvRepositorio.Setup(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>())).Returns(30000);
        }

        [Test]
        public void TestActividadConDescuentos()
        {
            srvRepositorio.Setup(x => x.DescuentaPesoDescontado("11-12")).Returns(true);
            srvRepositorio.Setup(x => x.EsRecorridoSustentable(It.IsAny<Guid>())).Returns(true);
            srvComandos.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(x => x.ValidaStockEPA(It.IsAny<Guid>())).Returns(true);

            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(x => x.DescuentaPesoDescontado("11-12"), Times.Once());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoConDescuento(It.IsAny<Guid>()), Times.Once());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>()), Times.Never());
            srvRepositorio.Verify(x => x.EsRecorridoSustentable(It.IsAny<Guid>()), Times.Once());
            srvComandos.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
        }

        [Test]
        public void TestActividadSinDescuentos()
        {
            srvRepositorio.Setup(x => x.DescuentaPesoDescontado("11-12")).Returns(false);
            srvComandos.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(x => x.EsRecorridoSustentable(It.IsAny<Guid>())).Returns(true);
            srvRepositorio.Setup(x => x.ValidaStockEPA(It.IsAny<Guid>())).Returns(true);
            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(x => x.DescuentaPesoDescontado("11-12"), Times.Once());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoConDescuento(It.IsAny<Guid>()), Times.Never());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>()), Times.Once());
            srvRepositorio.Verify(x => x.EsRecorridoSustentable(It.IsAny<Guid>()), Times.Once());
            srvComandos.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Once());
        }

        [Test]
        public void TestActividadFalla()
        {
            srvRepositorio.Setup(x => x.DescuentaPesoDescontado("11-12")).Returns(false);
            srvRepositorio.Setup(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>())).Throws(new Exception());
            srvComandos.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(x => x.EsRecorridoSustentable(It.IsAny<Guid>())).Returns(true);
            srvRepositorio.Setup(x => x.ValidaStockEPA(It.IsAny<Guid>())).Returns(true);
            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(x => x.DescuentaPesoDescontado("11-12"), Times.Once());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoConDescuento(It.IsAny<Guid>()), Times.Never());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>()), Times.Once());
            srvComandos.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Never());
            srvRepositorio.Verify(x => x.EsRecorridoSustentable(It.IsAny<Guid>()), Times.Once());
            Assert.That(resultado.Keys.Count, Is.EqualTo(1));
            Assert.That(resultado.Keys.Contains("Result"), Is.True);
        }

        [Test]
        public void TestActividadNoEsRecorridoSustentable()
        {
            srvRepositorio.Setup(x => x.DescuentaPesoDescontado("11-12")).Returns(false);
            srvRepositorio.Setup(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>())).Throws(new Exception());
            srvComandos.Setup(x => x.Ejecutar(It.IsAny<Comando>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(x => x.EsRecorridoSustentable(It.IsAny<Guid>())).Returns(false);
            srvRepositorio.Setup(x => x.ValidaStockEPA(It.IsAny<Guid>())).Returns(true);
            var resultado = host.TestActivity();

            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(x => x.DescuentaPesoDescontado("11-12"), Times.Never());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoConDescuento(It.IsAny<Guid>()), Times.Never());
            srvRepositorio.Verify(x => x.ObtenerPesoNetoSinDescuento(It.IsAny<Guid>()), Times.Never());
            srvComandos.Verify(x => x.Ejecutar(It.IsAny<Comando>()), Times.Never());
            srvRepositorio.Verify(x => x.EsRecorridoSustentable(It.IsAny<Guid>()), Times.Once());
            Assert.That(resultado.Keys.Count, Is.EqualTo(1));
            Assert.That(resultado.Keys.Contains("Result"), Is.True);
        }
    }
}
