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
    public class ProcesadorEliminarDescuentoTest
    {
        private ProcesadorEliminarDescuento target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private DescuentoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarDescuento(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new DescuentoDto
            {
                Id = 1,
                PorcentajeDescuento = 10,
                ValorHasta = 20,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarDescuento { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Descuento>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
