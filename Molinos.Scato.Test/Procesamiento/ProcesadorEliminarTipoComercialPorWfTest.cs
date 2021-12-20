using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarTipoComercialPorWfTest
    {
        private ProcesadorEliminarTipoComercialPorWf target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTipoComercialPorWf(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Workflow>(It.IsAny<int>())).Returns(new Workflow { TiposComercialesAsociados = new List<TipoComercial>() });
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(new TipoComercial());
            var comando = new EliminarTipoComercialPorWf { WorkflowId = 1, TipoComercialId = 1};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
