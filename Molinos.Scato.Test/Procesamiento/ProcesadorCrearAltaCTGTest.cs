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
    public class ProcesadorCrearAltaCTGTest
    {
        private ProcesadorCrearAltaCTG target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private AltaCTGDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAltaCTG(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new AltaCTGDto
                {
                    Id = 1,
                    CodigoCTG = "a",
                    Fecha = DateTime.Now,
                    CartaPorteId = 10,
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<CartaPorte>(It.IsAny<int>())).Returns(new CartaPorte { Id = tipoDto.CartaPorteId });
            var comando = new CrearAltaCTG { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<AltaCTG>(o => o.CodigoCTG == tipoDto.CodigoCTG)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}