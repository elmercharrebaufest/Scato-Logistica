using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorEliminarKmPorProveedorTest
    {
        private ProcesadorEliminarKmPorProveedor target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private KmPorProveedorDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarKmPorProveedor(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new KmPorProveedorDto
            {
                Id = 1,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarKmPorProveedor { Id = tipoDto.Id };
            repositorioMock.Setup(s => s.Obtener<KmPorProveedor>(It.IsAny<int>())).Returns(new KmPorProveedor());
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
