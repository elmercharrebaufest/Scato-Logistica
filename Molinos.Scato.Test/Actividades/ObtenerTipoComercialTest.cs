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
    public class ObtenerTipoComercialTest
    {
        private ObtenerTipoComercial target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new ObtenerTipoComercial();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestObtenerTipoComercial()
        {
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, PesoEsperado = 2, ToleranciaDifPesoE = 3});
            host.InArguments.TipoComercialId = 1;

            var resultado = host.TestActivity();

            var tipoComercial = resultado.First(f => f.Key == "TipoComercial").Value as TipoComercialDto;
            var pesoEsperado = resultado.First(f => f.Key == "PesoEsperado").Value;
            var tolerancia = resultado.First(f => f.Key == "Tolerancia").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(tipoComercial.Id, Is.EqualTo(1));
            Assert.That(pesoEsperado, Is.EqualTo(2));
            Assert.That(tolerancia, Is.EqualTo(3));
        }
    }
}
