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
    public class VerificarKilosDeclaradosPorFincaTest
    {
        private VerificarKilosDeclaradosPorFinca target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarKilosDeclaradosPorFinca();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarKilosDeclaradosPorFinca()
        {
            srvRepositorio.Setup(s => s.VerificarKilosDeclaradosPorVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new VerificarKilosDeclaradosPorFincaDto() { ExcedeKilosARecibir = true, AvisoDeCorte = true, KilosARecibir = 10,Vinedo = "a",Variedad = "b"});
            host.InArguments.VinedoId = 1;
            host.InArguments.MaterialId = 2;
            host.InArguments.Cosecha = "2013";
     

            var resultado = host.TestActivity();

            var excedeKilosARecibir = resultado.First(f => f.Key == "ExcedeKilosARecibir").Value;
            var avisoDeCorte = resultado.First(f => f.Key == "AvisoDeCorte").Value;
            var kilosARecibir = resultado.First(f => f.Key == "KilosARecibir").Value;
            var vinedo = resultado.First(f => f.Key == "Vinedo").Value;
            var variedad = resultado.First(f => f.Key == "Variedad").Value;


            Assert.That(resultado, Is.Not.Null);
            Assert.That(excedeKilosARecibir, Is.EqualTo(true));
            Assert.That(avisoDeCorte, Is.EqualTo(true));
            Assert.That(kilosARecibir, Is.EqualTo(10));
            Assert.That(vinedo, Is.EqualTo("a"));
            Assert.That(variedad, Is.EqualTo("b"));
        }

    }
}
