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
    public class ProcesadorEliminarEntregadorTest
    {
        private ProcesadorEliminarEntregador target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private EntregadorDto entregadorDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarEntregador(repositorioMock.Object, conversorMock.Object, new NullLogger());
            entregadorDto = new EntregadorDto
            {
                Id = 1,
                CodigoSAPCondicionFiscal = "1",
                DescripcionCorta = "Codigo Cero",
                Cuil = "20-30591238-8",
                Domicilio = "Domicilio",
                TipoEntregador = "Tipo",
                RazonSocial = "Emilio",
                Tratamiento = "Tratamiento",
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarEntregador { Id = entregadorDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Entregador>(It.Is<object>(o => (int)o == entregadorDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
