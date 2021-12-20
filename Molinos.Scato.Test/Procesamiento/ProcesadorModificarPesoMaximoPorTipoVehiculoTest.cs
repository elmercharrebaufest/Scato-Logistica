using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarPesoMaximoPorTipoVehiculoTest
    {
        private ProcesadorModificarPesoMaximoPorTipoVehiculo target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private PesoMaximoPorTipoVehiculoDto tipoDto;
        private PesoMaximoPorTipoVehiculo tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarPesoMaximoPorTipoVehiculo(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new PesoMaximoPorTipoVehiculoDto()
            {
                Id = 5,
                TipoVehiculo = TipoVehiculo.Tren,
                PesoMaxIngreso = 11,
                PesoMaxEgreso = 12,
                Activo = true,
                CentroId = 1
            };
            tipo = new PesoMaximoPorTipoVehiculo
                {
                    Id = 5,
                    TipoVehiculo = TipoVehiculo.Tren,
                    PesoMaxIngreso = 11,
                    PesoMaxEgreso = 12,
                    Activo = true,
                    Centro = new Centro()
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var comando = new ModificarPesoMaximoPorTipoVehiculo() { Dto = tipoDto };
            repositorioMock.Setup(s => s.Obtener<PesoMaximoPorTipoVehiculo>(It.IsAny<int>())).Returns(tipo);
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<PesoMaximoPorTipoVehiculo>(o => o.TipoVehiculo == tipoDto.TipoVehiculo)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<PesoMaximoPorTipoVehiculo>
                {
                    new PesoMaximoPorTipoVehiculo() {Id = 5, TipoVehiculo = TipoVehiculo.Camión, Centro = new Centro{Id = 1}},
                    new PesoMaximoPorTipoVehiculo() {Id = 6, TipoVehiculo = TipoVehiculo.Tren, Centro = new Centro{Id = 1}},
                };
            repositorioMock.Setup(s => s.Obtener<PesoMaximoPorTipoVehiculo>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>()))
                    .Returns<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new ModificarPesoMaximoPorTipoVehiculo() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo(""));
        }

        [Test]
        public void TestModificarEntidadValido2()
        {
            repositorioMock.Setup(s => s.Obtener<PesoMaximoPorTipoVehiculo>(It.IsAny<int>())).Returns(tipo);
            var tiposExistentes = new List<PesoMaximoPorTipoVehiculo>
                {
                    new PesoMaximoPorTipoVehiculo() {Id = 5, TipoVehiculo = TipoVehiculo.Camión, Centro = new Centro{Id = 1}},
                    new PesoMaximoPorTipoVehiculo() {Id = 6, TipoVehiculo = TipoVehiculo.Tren, Centro = new Centro{Id = 1}},
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>()))
                    .Returns<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.CentroId = 2;

            var comando = new ModificarPesoMaximoPorTipoVehiculo() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestModificarEntidadValido3()
        {
            repositorioMock.Setup(s => s.Obtener<PesoMaximoPorTipoVehiculo>(It.IsAny<int>())).Returns(tipo);
            var tiposExistentes = new List<PesoMaximoPorTipoVehiculo>
                {
                    new PesoMaximoPorTipoVehiculo() {Id = 6, TipoVehiculo = TipoVehiculo.Camión, Centro = new Centro{Id = 1}},
                    new PesoMaximoPorTipoVehiculo() {Id = 5, TipoVehiculo = TipoVehiculo.Tren, Centro = new Centro{Id = 1}},
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>()))
                    .Returns<Expression<Func<PesoMaximoPorTipoVehiculo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.PesoMaxEgreso = 100;
            var comando = new ModificarPesoMaximoPorTipoVehiculo() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Errores.Count, Is.EqualTo(0));
        }
    }
}
