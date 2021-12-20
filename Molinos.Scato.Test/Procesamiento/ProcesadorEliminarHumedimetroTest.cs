using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarHumedimetroTest
    {
        private ProcesadorEliminarHumedimetro target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private HumedimetroDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarHumedimetro(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new HumedimetroDto
            {
                Id = 1,
                Modalidad = Modalidad.Automática,
                Descripcion = "Humedimetro 1",
                Codigo = "1",
                CentroId = 1
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarHumedimetro { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Humedimetro>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
