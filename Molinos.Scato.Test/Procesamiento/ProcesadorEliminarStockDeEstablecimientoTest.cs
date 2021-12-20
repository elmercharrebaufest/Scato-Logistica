using System;
using System.Collections.Generic;
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
    public class ProcesadorEliminarStockDeEstablecimientoTest
    {
        private ProcesadorEliminarStockDeEstablecimiento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private StockDeEstablecimientoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarStockDeEstablecimiento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new StockDeEstablecimientoDto
            {
                Id = 1,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarStockDeEstablecimiento {Id = tipoDto.Id};
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(new List<decimal>());
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<StockDeEstablecimiento>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEliminarEntidadError()
        {
            var comando = new EliminarStockDeEstablecimiento { Id = tipoDto.Id };
            repositorioMock.Setup(s => s.Sumar<RegistroStockEPA>(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(31000);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<StockDeEstablecimiento>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
