using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProcesadorModificarStockDeEstablecimientoTest
    {
        private ProcesadorModificarStockDeEstablecimiento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private StockDeEstablecimientoDto tipoDto;
        private StockDeEstablecimiento tipo, tipo2;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarStockDeEstablecimiento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new StockDeEstablecimientoDto
            {
                Id = 1,
                CodigoEstablecimiento = "101",
                Cosecha = "15-16",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(2016, 11, 15),
                StockDeclarado = 4000,
                StockUtilizado = 0
            };
            tipo = new StockDeEstablecimiento
            {
                Id = 1,
                CodigoEstablecimiento = "101",
                Cosecha = "15-16",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(2016,11,15),
                StockDeclarado = 5000
            };
            tipo2 = new StockDeEstablecimiento
            {
                Id = 2,
                CodigoEstablecimiento = "101",
                Cosecha = "15-16",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(2016, 11, 11),
                StockDeclarado = 5000
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<StockDeEstablecimiento>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(new List<decimal>());
            repositorioMock.Setup(s => s.ObtenerProyeccion<StockDeEstablecimiento, DateTime>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>(), It.IsAny<Expression<Func<StockDeEstablecimiento, DateTime>>>())).Returns(new DateTime(2016, 11, 15));
            var comando = new ModificarStockDeEstablecimiento {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<StockDeEstablecimiento>
                {
                    tipo2
                };
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(new List<decimal>());
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>()))
                           .Returns<Expression<Func<StockDeEstablecimiento, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<StockDeEstablecimiento>(It.IsAny<int>())).Returns(tipo2);
            repositorioMock.Setup(s => s.ObtenerProyeccion<StockDeEstablecimiento, DateTime>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>(), It.IsAny<Expression<Func<StockDeEstablecimiento, DateTime>>>())).Returns(new DateTime(2016, 11, 16));
            var comando = new ModificarStockDeEstablecimiento { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestModificarEntidadError2()
        {
            repositorioMock.Setup(s => s.Obtener<StockDeEstablecimiento>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Sumar<RegistroStockEPA>(It.IsAny<Expression<Func<RegistroStockEPA, decimal>>>(), It.IsAny<Expression<Func<RegistroStockEPA, bool>>>())).Returns(1056m);
            repositorioMock.Setup(s => s.ObtenerProyeccion<StockDeEstablecimiento, DateTime>(It.IsAny<Expression<Func<StockDeEstablecimiento, bool>>>(), It.IsAny<Expression<Func<StockDeEstablecimiento, DateTime>>>())).Returns(new DateTime(2016, 11, 15));
            tipoDto.StockDeclarado = 500;
            var comando = new ModificarStockDeEstablecimiento { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}