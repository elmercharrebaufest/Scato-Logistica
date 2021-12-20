using System;
using System.Linq.Expressions;
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
    public class ProcesadorCrearStockDeEstablecimientoTest
    {
        private ProcesadorCrearStockDeEstablecimiento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private StockDeEstablecimientoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearStockDeEstablecimiento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new StockDeEstablecimientoDto
                {
                    Id = 1,
                    CodigoEstablecimiento = "101",
                    Cosecha = "15-16",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    StockDeclarado = 5000,
                    StockUtilizado = 0
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearStockDeEstablecimiento{Dto = tipoDto};
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Establecimiento, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<StockDeEstablecimiento>(o => o.CodigoEstablecimiento == tipoDto.CodigoEstablecimiento)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadError()
        {
            var comando = new CrearStockDeEstablecimiento { Dto = tipoDto };
            repositorioMock.Setup(s => s.Existe<StockDeEstablecimiento>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>())).Returns(true);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<StockDeEstablecimiento>(o => o.CodigoEstablecimiento == tipoDto.CodigoEstablecimiento)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
        }

    }
}