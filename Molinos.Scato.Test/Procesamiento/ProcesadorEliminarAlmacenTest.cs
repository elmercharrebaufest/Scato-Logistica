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
    public class ProcesadorEliminarAlmacenTest
    {
        private ProcesadorEliminarAlmacen target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private AlmacenDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarAlmacen(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new AlmacenDto
            {
                Id = 1,
                Descripcion = "Almacen 1",
                DescripcionCorta = "Alm 1",
                CentroId = 1,
                CodigoSAP = "1",
                CodigoONCCA = "1",
                EsTanqueVino = false
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarAlmacen { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Almacen>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
