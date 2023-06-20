using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarCalleTest
    {
        private ProcesadorModificarCalle target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private Mock<IServicioComandos> comandosMock;
        private CalleDto tipoDto;
        private Calle tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarCalle(repositorioMock.Object, conversor, new NullLogger(), comandosMock.Object);
            tipoDto = new CalleDto()
            {
                Id = 1,
                Codigo = "C1",
                CentroId = 1,
                Nombre = "Calle Nro 1"
            };
            tipo = new Calle()
            {
                Id = 1,
                Codigo = "C1",
                CentroId = 1,
                Nombre = "Calle Nro 1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Calle>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarCalle() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
