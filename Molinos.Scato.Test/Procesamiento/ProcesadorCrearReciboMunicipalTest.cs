using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearReciboMunicipalTest
    {
        private ProcesadorCrearReciboMunicipal target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private ReciboMunicipalDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearReciboMunicipal(repositorioMock.Object, conversorMock, new NullLogger());
            tipoDto = new ReciboMunicipalDto
            {
                Id = 1,
                Monto = 50,
                Ordenanza = "Ordenanza",
                CentroId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearReciboMunicipal { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ReciboMunicipal>(o => o.Ordenanza == tipoDto.Ordenanza)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
