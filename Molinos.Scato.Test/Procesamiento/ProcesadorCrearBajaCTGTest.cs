using System;
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
    public class ProcesadorCrearBajaCTGTest
    {
        private ProcesadorCrearBajaCTG target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private BajaCTGDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearBajaCTG(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new BajaCTGDto
                {
                    Id = 1,
                    CodigoDeBaja = "a",
                    Fecha = DateTime.UtcNow,
                    CartaPorteId = 10,
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearBajaCTG {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<BajaCTG>(o => o.CodigoDeBaja == tipoDto.CodigoDeBaja)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}