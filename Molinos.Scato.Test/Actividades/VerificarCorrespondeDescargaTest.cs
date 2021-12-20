using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarCorrespondeDescargaTest
    {
        private VerificarCorrespondeDescarga target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarCorrespondeDescarga();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarCorrespondeDescarga()
        {
            srvRepositorio.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new MaterialPorCentroDto(){Id = 1, CentroId = 1, MaterialId = 1, CorrespondeDescarga = true});
            srvRepositorio.Setup(s => s.VerificarCorrespondeDescarga(It.IsAny<Guid>())).Returns(true);
            host.InArguments.InstanceId = new Guid();         

            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "CorrespondeDescarga").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(true));
        }

        [Test]
        public void TestVerificarCorrespondeDescargaFalso()
        {
            srvRepositorio.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new MaterialPorCentroDto() { Id = 1, CentroId = 1, MaterialId = 1, CorrespondeDescarga = false });
            srvRepositorio.Setup(s => s.VerificarCorrespondeDescarga(It.IsAny<Guid>())).Returns(false);
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "CorrespondeDescarga").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(false));
        }
    }
}
